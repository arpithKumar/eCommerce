using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;

namespace OrderApi.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersController(IOrder orderInterface, IOrderService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
    {
        var orders = await orderInterface.GetAllAsync();
        if(!orders.Any()) return NotFound("There are no orders");
        var (_, orderList) = OrderConversions.FromEntity(null, orders);
        return (!orderList!.Any()) ? NotFound("There are no orders") : Ok(orderList);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var order = await orderInterface.GetByIdAsync(id);
        return order is null ? NotFound(null) : Ok(OrderConversions.FromEntity(order, null));
    }

    [HttpGet("client/{clientId:int}")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetClientOrders(int clientId)
    {
        if(clientId <= 0) return BadRequest("Invalid clientId");
        var orders = await service.GetOrdersByClientId(clientId);
        return orders is null ? NotFound("There are no orders") : Ok(orders);
    }

    [HttpGet("details/{orderId:int}")]
    public async Task<ActionResult<OrderDetailsDto>> GetOrderDetails(int orderId)
    {
        if(orderId <= 0) return BadRequest("Invalid orderId");
        var orderDetails = await service.GetOrderDetails(orderId);
        return orderDetails.OrderId > 0 ? Ok(orderDetails) : NotFound("There are no orders");
    }

    [HttpPost]
    public async Task<ActionResult<Response>> CreateOrder(OrderDto orderDto)
    {
        if(!ModelState.IsValid) return BadRequest("Invalid order data");
        var entity = OrderConversions.ToEntity(orderDto);
        var response = await orderInterface.CreateAsync(entity);
        return response.Flag ? Ok(response) : BadRequest(response);
    }

    [HttpPut]
    public async Task<ActionResult<Response>> UpdateOrder(OrderDto orderDto)
    {
        var order = OrderConversions.ToEntity(orderDto);
        var response = await orderInterface.UpdateAsync(order);
        return response.Flag ? Ok(response) : BadRequest(response);
    }

    [HttpDelete]
    public async Task<ActionResult<Response>> DeleteOrder(OrderDto orderDto)
    {
        var order = OrderConversions.ToEntity(orderDto);
        var response = await orderInterface.DeleteAsync(order);
        return response.Flag ? Ok(response) : BadRequest(response);
    }
    
}