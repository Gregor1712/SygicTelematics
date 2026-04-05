namespace Alert.Application.Interfaces;

public interface IEmailService
{
    Task SendAlertEmailAsync(string alertType, string message, Guid vehicleId, DateTime timestamp);
}
