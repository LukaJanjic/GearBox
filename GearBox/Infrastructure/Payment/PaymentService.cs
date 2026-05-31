using Core.Exceptions;
using Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Payment;

public class PaymentService(ICartService cartService, IConfiguration config) : IPaymentService
{
    public async Task<string> CreatePaymentIntentAsync(string? userId, decimal? guestAmount = null)
    {
        StripeConfiguration.ApiKey = config["Stripe:SecretKey"];

        long amount;
        var  metadata = new Dictionary<string, string>();

        if (userId is not null)
        {
            var cart = await cartService.GetCartAsync(userId);
            if (cart is null || cart.Items.Count == 0)
                throw new BadRequestException("Cart is empty.");

            amount = (long)(cart.Total * 0.9m * 100); // 10% member discount
            metadata["userId"] = userId;
        }
        else
        {
            if (guestAmount is null or <= 0)
                throw new BadRequestException("Invalid cart amount.");

            amount = (long)(guestAmount.Value * 100);
        }

        var options = new PaymentIntentCreateOptions
        {
            Amount             = amount,
            Currency           = "eur",
            PaymentMethodTypes = ["card"],
            Metadata           = metadata,
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
