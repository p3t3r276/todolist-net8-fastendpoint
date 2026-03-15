using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FastTodo.Domain.Shared;

namespace FastTodo;

public static class QueryableExtension
{
    public static IQueryable<T> OrderByName<T>(this IQueryable<T> source, string propertyName, bool isDescending)
    {
        return ApplyOrder(source, propertyName, isDescending);
    }

    private static IQueryable<T> ApplyOrder<T>(IQueryable<T> source, string propertyName, bool isDescending, bool isNextOrder = false)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException("Order by property should not empty", nameof(propertyName));
        }

        var parameter = Expression.Parameter(typeof(T), "p");

        var propertyParts = propertyName.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var propertyAccess = propertyParts.Length > 0 ?
            propertyParts.Aggregate<string, Expression>(parameter, Expression.PropertyOrField)
            : Expression.PropertyOrField(parameter, propertyName);

        var orderByExpression = Expression.Lambda(propertyAccess, parameter);
        var methodName = GetMethodName<T>(isDescending, isNextOrder);
        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new Type[] { typeof(T), propertyAccess.Type },
            source.Expression,
            Expression.Quote(orderByExpression));

        return source
            .Provider
            .CreateQuery<T>(resultExpression);
    }

    private static string GetMethodName<T>(bool isDescending, bool isNextOrder = false)
    {
        if (isNextOrder)
        {
            return isDescending ? "ThenByDescending" : "ThenBy";
        }

        return isDescending ? "OrderByDescending" : "OrderBy";
    }

    public static IQueryable<T> ThenOrderByName<T>(this IQueryable<T> source, string propertyName, bool isDescending)
    {
        return ApplyOrder(source, propertyName, isDescending, true);
    }

    public static async ValueTask<List<TSource>> ToListEfAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellation)
    {
        if (source is not IAsyncEnumerable<TSource> asyncEnumerable)
        {
            return await Task.Run(() => source.ToList());
        }

        var list = new List<TSource>();
        await foreach (var element in asyncEnumerable.WithCancellation(cancellation).ConfigureAwait(false))
        {
            list.Add(element);
        }

        return list;
    }

    public static async ValueTask<ImmutableArray<TSource>> ToImmutableArrayAsync<TSource>(
        this IQueryable<TSource> source,
        CancellationToken cancellation)
    {
        if (source is not IAsyncEnumerable<TSource> asyncEnumerable)
        {
            throw new InvalidOperationException("IQueryable is not async");
        }

        var builder = ImmutableArray.CreateBuilder<TSource>();
        await foreach (var element in asyncEnumerable.WithCancellation(cancellation).ConfigureAwait(false))
        {
            builder.Add(element);
        }

        return builder.ToImmutable();
    }

    public static async Task<int> CountAsync<T>(this IQueryable<T> source, CancellationToken cancellationToken = default)
    {
        return source == null ? throw new ArgumentNullException(nameof(source))
            : await Task.Run(() => source.Select(x => 1).Count(), cancellationToken);
    }

    public static async Task<PaginatedList<TResult>> AsPagination<TResult>(this IQueryable<TResult> source, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        where TResult : notnull
    {
        var totalCount = await source.Select(x => 1).CountAsync(cancellationToken);
        // var paging = new Paging(totalCount, pageNumber, pageSize);

        var pageIndex = pageNumber - 1;

        var result = await source.Skip((pageIndex < 0 ? 0 : pageIndex) * pageSize)
            .Take(pageSize)
            .ToListEfAsync(cancellationToken);

        return new PaginatedList<TResult>(result, totalCount, pageIndex, pageSize);
    }

    public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> queryable, bool condition, Expression<Func<TSource, bool>> predicate)
    {
        if (condition)
        {
            queryable = queryable.Where(predicate);
        }

        return queryable;
    }
}
