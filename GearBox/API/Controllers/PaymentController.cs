using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

public record GuestPaymentRequest(decimal Amount);

[Route("api/[controller]")]
[ApiController]
public class PaymentController(IPaymentService paymentService) : ControllerBase
{
    [HttpPost("create-intent")]
    [AllowAnonymous]
    public async Task<ActionResult<string>> CreateIntent([FromBody] GuestPaymentRequest? request)
    {
        var userId       = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var clientSecret = await paymentService.CreatePaymentIntentAsync(userId, request?.Amount);
        return Ok(new { clientSecret });
    }

    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<ActionResult> Webhook()
    {
        var json             = await new StreamReader(Request.Body).ReadToEndAsync();
        var stripeSignature  = Request.Headers["Stripe-Signature"].ToString();

        await paymentService.HandleWebhookAsync(json, stripeSignature);
        return Ok();
    }
}
