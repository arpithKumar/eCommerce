using OrderApi.Domain.Entities;

namespace OrderApi.Application.DTOs.Conversions;

public static class OrderConversions
{
    public static Order ToEntity(OrderDto dto) => new()
    {
        Id = dto.Id,
        ClientId = dto.ClientId,
        OrderedDate = dto.OrderedDate,
        ProductId = dto.ProductId,
        PurchaseQuantity = dto.PurchaseQuantity
    };

    public static (OrderDto?, IEnumerable<OrderDto>?) FromEntity(Order? order, IEnumerable<Order>? orders)
    {
        if (order is not null || orders is null)
        {
            return (new OrderDto(
                order!.Id,
                order.ProductId,
                order.ClientId,
                order.PurchaseQuantity,
                order.OrderedDate
                ),null);
        }

        if (orders is not null || order is null)
        {
            return (null, orders!.Select(or => new OrderDto(or.Id, or.ProductId, or.ClientId, or.PurchaseQuantity, or.OrderedDate)).ToList());
        }

        return (null, null);
    }
}