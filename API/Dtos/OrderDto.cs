﻿using API.Dtos;

namespace API;

public class OrderDto
{
    public string BasketId { get; set; }
    public int DeliveryMethodId { get; set; }
    public AddressDto ShippingAddress { get; set; }

}