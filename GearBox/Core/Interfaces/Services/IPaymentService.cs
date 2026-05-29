namespace Core.Interfaces.Services;

public interface IPaymentService
{
    Task<string> CreatePaymentIntentAsync(string userId);
    Task HandleWebhookAsync(string json, string stripeSignature);
}
