using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FastTodo.Infrastructure.Domain.ValueConverion;

public class DateTimeToDateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTime>
{
    public DateTimeToDateTimeOffsetConverter() : base(
        dateTimeOffset => dateTimeOffset.UtcDateTime,     // Convert back to DateTime
        dateTime => new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc))     // Convert to DateTimeOffset
    ) { }
}
