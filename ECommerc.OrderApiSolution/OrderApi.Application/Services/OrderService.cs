using System.Net.Http.Json;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using Polly.Registry;

namespace OrderApi.Application.Services;

public class OrderService(IOrder orderInterface, HttpClient httpClient, ResiliencePipelineProvider<string> resiliencePipelineProvider) : IOrderService
{
    public async Task<ProductDto> GetProductById(int productId)
    {
        var response = await httpClient.GetAsync($"api/products/{productId}");
        if (!response.IsSuccessStatusCode) return null!;
        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        return product!;
    }

    public async Task<AppUserDto> GetUserById(int userId)
    {
        var response = await httpClient.GetAsync($"api/authentication/{userId}");
        if (!response.IsSuccessStatusCode) return null!;
        var user = await response.Content.ReadFromJsonAsync<AppUserDto>();
        return user!; 
    }
    
    public async Task<IEnumerable<OrderDto>> GetOrdersByClientId(int clientId)
    {
        var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);
        if(!orders.Any()) return null!;
        var (_, entities) = OrderConversions.FromEntity(null, orders);
        return entities!;
    }

    public async Task<OrderDetailsDto> GetOrderDetails(int orderId)
    {
        var order = await orderInterface.GetByIdAsync(orderId);
        if(order is null || order!.Id <= 0) return null!;
        var retryPipeline = resiliencePipelineProvider.GetPipeline("order-retry-pipeline");
        var productDto = await retryPipeline.ExecuteAsync(async token => await GetProductById(order.ProductId));
        var appUserDto = await retryPipeline.ExecuteAsync(async token => await GetUserById(order.ClientId));
        return new OrderDetailsDto(
            order.Id,
            productDto.Id,
            appUserDto.Id,
            appUserDto.Name,
            appUserDto.Email,
            appUserDto.Address,
            appUserDto.TelephoneNumber,
            productDto.Name,
            order.PurchaseQuantity,
            productDto.Price,
            productDto.Quantity * order.PurchaseQuantity,
            order.OrderedDate
            );
    }
}