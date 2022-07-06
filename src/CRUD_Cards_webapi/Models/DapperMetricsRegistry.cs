using App.Metrics;
using App.Metrics.Counter;

namespace CRUD_Cards_webapi.Models;

public sealed class DapperMetricsRegistry
{
    public static CounterOptions CreatedDebetCardsCounter = new()
    {
        Name = "Created Debet Cards",
        Context = "DAPPER",
        MeasurementUnit = Unit.Calls
    };
}