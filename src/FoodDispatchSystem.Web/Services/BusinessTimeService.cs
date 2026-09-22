namespace FoodDispatchSystem.Web.Services;

public class BusinessTimeService
{
    private readonly TimeZoneInfo _timeZone;

    public BusinessTimeService(IConfiguration configuration)
    {
        var configuredTimeZone =
            configuration["BusinessTimeZone"]
            ?? "America/El_Salvador";

        _timeZone = ResolveTimeZone(configuredTimeZone);
    }

    public DateTime UtcNow =>
        DateTime.UtcNow;

    public DateTime LocalNow =>
        ToLocalTime(DateTime.UtcNow);

    public DateTime LocalToday =>
        LocalNow.Date;

    public DateTime ToLocalTime(DateTime utcDateTime)
    {
        var utc = DateTime.SpecifyKind(
            utcDateTime,
            DateTimeKind.Utc);

        return TimeZoneInfo.ConvertTimeFromUtc(
            utc,
            _timeZone);
    }

    public DateTime ToUtc(DateTime localDateTime)
    {
        var local = DateTime.SpecifyKind(
            localDateTime,
            DateTimeKind.Unspecified);

        return TimeZoneInfo.ConvertTimeToUtc(
            local,
            _timeZone);
    }

    public (DateTime StartUtc, DateTime EndUtc)
        GetUtcRangeForLocalDate(DateTime localDate)
    {
        var startLocal = DateTime.SpecifyKind(
            localDate.Date,
            DateTimeKind.Unspecified);

        var endLocal = startLocal.AddDays(1);

        var startUtc =
            TimeZoneInfo.ConvertTimeToUtc(
                startLocal,
                _timeZone);

        var endUtc =
            TimeZoneInfo.ConvertTimeToUtc(
                endLocal,
                _timeZone);

        return (startUtc, endUtc);
    }

    private static TimeZoneInfo ResolveTimeZone(
        string configuredTimeZone)
    {
        var candidates = new[]
        {
            configuredTimeZone,
            "America/El_Salvador",
            "Central America Standard Time"
        };

        foreach (var candidate in candidates.Distinct())
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(
                    candidate);
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        throw new InvalidOperationException(
            "No se pudo configurar la zona horaria del negocio.");
    }
}