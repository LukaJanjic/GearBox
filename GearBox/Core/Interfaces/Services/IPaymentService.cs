namespace Core.Interfaces.Services;

public interface IPaymentService
{
    Task<string> CreatePaymentIntentAsync(string? userId, decimal? guestAmount = null);
    Task HandleWebhookAsync(string json, string stripeSignature);
}
