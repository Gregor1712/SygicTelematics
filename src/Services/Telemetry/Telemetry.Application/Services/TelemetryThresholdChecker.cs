using Shared.Kernel.Events;

namespace Telemetry.Application.Services;

public static class TelemetryThresholdChecker
{
    private static readonly Dictionary<string, (double Threshold, string Comparison, string AlertType, string Message)> Rules = new()
    {
        ["engine_temp"] = (100.0, "above", "HIGH_TEMP", "Engine temperature above 100°C"),
        ["coolant_temp"] = (105.0, "above", "HIGH_TEMP", "Coolant temperature above 105°C"),
        ["oil_pressure"] = (1.0, "below", "LOW_OIL_PRESSURE", "Oil pressure critically low"),
        ["tire_pressure_fl"] = (1.8, "below", "LOW_TIRE_PRESSURE", "Front left tire pressure below safe level"),
        ["tire_pressure_fr"] = (1.8, "below", "LOW_TIRE_PRESSURE", "Front right tire pressure below safe level"),
        ["tire_pressure_rl"] = (1.8, "below", "LOW_TIRE_PRESSURE", "Rear left tire pressure below safe level"),
        ["tire_pressure_rr"] = (1.8, "below", "LOW_TIRE_PRESSURE", "Rear right tire pressure below safe level"),
        ["rpm"] = (6000.0, "above", "HIGH_RPM", "Engine RPM exceeded safe limit"),
    };

    public static TelemetryAlertEvent? Check(Guid vehicleId, string type, double value, DateTime timestamp)
    {
        if (!Rules.TryGetValue(type, out var rule))
            return null;

        var exceeded = rule.Comparison == "above"
            ? value > rule.Threshold
            : value < rule.Threshold;

        if (!exceeded)
            return null;

        return new TelemetryAlertEvent
        {
            VehicleId = vehicleId,
            AlertType = rule.AlertType,
            Message = $"{rule.Message} (value: {value:F2})",
            TelemetryType = type,
            Value = value,
            Timestamp = timestamp
        };
    }
}
