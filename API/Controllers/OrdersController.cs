using System.Security.Claims;
using API.Controllers;
using API.Dtos;
using API.Errors;
using AutoMapper;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API;

[Authorize]
public class OrdersController : BaseApiController
{
    private readonly IOrderService _orderService;
    private readonly IMapper _mapper;
    public OrdersController(IOrderService orderService, IMapper mapper)
    {
      this._mapper = mapper;
      this._orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
    {
      var email = HttpContext.User.RetrieveEmailFromPrincipal();
      var address = _mapper.Map<AddressDto, Address>(orderDto.ShippingAddress);
      var order = await _orderService.CreateOrderAsync(email, orderDto.DeliveryMethodId, orderDto.BasketId, address);

      if(order == null) return BadRequest(new ApiResponse(400, "There was a problem processing your order"));

      return Ok(order);

    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Order>>> GetOrdersForUser()
    {
      var email = HttpContext.User.RetrieveEmailFromPrincipal(); 
      var orders = await _orderService.GetOrdersForUserAsync(email); 

      return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrderByIdForUser(int id)
    {
      var email = HttpContext.User.RetrieveEmailFromPrincipal(); 
      var order = await _orderService.GetOrderByIdAsync(id, email); 

      if(order == null) return NotFound(new ApiResponse(404));

      return Ok(order);
    }

    [HttpGet("deliveryMethods")]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
    {
      return Ok(await _orderService.GetDeliveryMethodsAsync());
    }

}
