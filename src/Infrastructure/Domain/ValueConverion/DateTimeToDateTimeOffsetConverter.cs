using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FastTodo.Infrastructure.Domain.ValueConverion;

public class DateTimeToDateTimeOffsetConverter : ValueConverter<DateTime, DateTimeOffset>
{
    public DateTimeToDateTimeOffsetConverter() : base(
        dateTime => new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)),     // Convert to DateTimeOffset
        dateTimeOffset => dateTimeOffset.UtcDateTime     // Convert back to DateTime
    )
    { }
}