using Core.Exceptions;
using Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Payment;

public class PaymentService(ICartService cartService, IConfiguration config) : IPaymentService
{
    public async Task<string> CreatePaymentIntentAsync(string userId)
    {
        StripeConfiguration.ApiKey = config["Stripe:SecretKey"];

        var cart = await cartService.GetCartAsync(userId);
        if (cart is null || cart.Items.Count == 0)
            throw new BadRequestException("Cart is empty.");

        var amount = (long)(cart.Total * 100);

        var options = new PaymentIntentCreateOptions
        {
            Amount   = amount,
            Currency = "eur",
            PaymentMethodTypes = ["card"],
            Metadata = new Dictionary<string, string> { ["userId"] = userId }
        };

        var service = new PaymentIntentService();
        var intent  = await service.CreateAsync(options);

        return intent.ClientSecret;
    }

    public async Task HandleWebhookAsync(string json, string stripeSignature)
    {
        var webhookSecret = config["Stripe:WebhookSecret"];

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, webhookSecret);
        }
        catch
        {
            throw new BadRequestException("Invalid Stripe webhook signature.");
        }

        if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
        {
            var intent = stripeEvent.Data.Object as PaymentIntent;
            // TODO: kreirati Order kada se doda Order modul
        }

        await Task.CompletedTask;
    }
}
