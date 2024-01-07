using Core;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure;

public class PaymentService : IPaymentService
{
  private readonly IBasketRepository _basketRepository;
  private readonly IUnitOfWork _unitOfWork;
  private readonly IConfiguration _config;


  public PaymentService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IConfiguration config)
  {
    _basketRepository = basketRepository;
    _unitOfWork = unitOfWork;
    _config = config;
  } 

  public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
  {
    // Set up API key configuration
    StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];
    
    // Get basket from repo
    var basket = await _basketRepository.GetBasketAsync(basketId);

    if(basket == null) return null;

    // Set up shipping price
    var shippingPrice = 0m;
    if(basket.DeliveryMethodId.HasValue)
    {
      var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>()
        .GetByIdAsync((int)basket.DeliveryMethodId);
      shippingPrice = deliveryMethod.Price;
    }

    // Verify const of items. 
    foreach (var item in basket.Items)
    {
      var productItem = await _unitOfWork.Repository<Core.Entities.Product>().GetByIdAsync(item.Id);
      if(item.Price != productItem.Price)
      {
        item.Price = productItem.Price;
      }
    }

    // Set up payment intent
    var service = new PaymentIntentService();
    PaymentIntent intent;

    var CartAmount = (long) basket.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long) shippingPrice * 100;

    if(string.IsNullOrEmpty(basket.PaymentIntentId))
    {
      var options = new PaymentIntentCreateOptions{
        Amount = CartAmount,
        Currency = "usd",
        PaymentMethodTypes = new List<string>{"card"}
      };
      intent = await service.CreateAsync(options);
      basket.PaymentIntentId = intent.Id;
      basket.ClientSecret = intent.ClientSecret;
    }
    else
    {
      var options = new PaymentIntentUpdateOptions
      {
        Amount = CartAmount
      };
      await service.UpdateAsync(basket.PaymentIntentId, options);
    }
      
    await _basketRepository.UpdateBasketAsync(basket);

    return basket;
  }

    public async Task<Order> UpdateOrderPaymentFailed(string paymentIntentId)
    {
        var spec = new OrderByPaymentIntentIdSpecification(paymentIntentId);
        var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

        if(order == null) return null;

        order.Status = OrderStatus.PaymentFailed;
        await _unitOfWork.Complete();

        return order;        
    }

    public async Task<Order> UpdateOrderPaymentSucceeded(string paymentIntentId)
    {
        var spec = new OrderByPaymentIntentIdSpecification(paymentIntentId);
        var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

        if(order == null) return null;

        order.Status = OrderStatus.PaymentReceived;
        await _unitOfWork.Complete();

        return order; 
    }
}
