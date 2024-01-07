using API.Controllers;
using API.Errors;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace API;

public class PaymentsController : BaseApiController
{
  private const string WhSecret = "whsec_dd665b2c3f708989329e36fecb71da89663c4326aef6647b1bd06a077eba4655";
  private readonly IPaymentService _paymentService;
  private readonly ILogger<IPaymentService> _logger;


  public PaymentsController(IPaymentService paymentService, ILogger<IPaymentService> logger)
  {
    _logger = logger;
    _paymentService = paymentService;
  }

  [Authorize]
  [HttpPost("{basketId}")]
  public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
  {
    var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
    if(basket == null)
    {
      return BadRequest(new ApiResponse(400,"Problem with your basket"));
    } 
      
    return basket;
  }

  [HttpPost("webhook")]
  public async Task<ActionResult> StripeWebhook()
  {
    var json = await new StreamReader(Request.Body).ReadToEndAsync();

    var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], WhSecret);

    PaymentIntent intent;
    Order order;

    switch(stripeEvent.Type)
    {
      case "payment_intent.succeeded":
        intent = (PaymentIntent)stripeEvent.Data.Object;
        _logger.LogInformation("Payment Succeeded: {IntentId}", intent.Id);
        order = await _paymentService.UpdateOrderPaymentSucceeded(intent.Id);
        _logger.LogInformation("Order updated to payment received: {OrderId}", order.Id);
        break;
      case "payment_intent.failed":
        intent = (PaymentIntent)stripeEvent.Data.Object;
        _logger.LogInformation("Payment Failed: {IntentId}", intent.Id);
        order = await _paymentService.UpdateOrderPaymentFailed(intent.Id);
        _logger.LogInformation("Order updated to payment received: {OrderId}", order.Id);
        break;
      default:
        _logger.LogInformation("Unhandled event type: {StripeEvent}", stripeEvent.Type);
        break;
    }

    return new EmptyResult();
  } 
}
