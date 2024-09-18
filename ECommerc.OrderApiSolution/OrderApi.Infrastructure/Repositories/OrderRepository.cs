using System.Linq.Expressions;
using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;

namespace OrderApi.Infrastructure.Repositories;

public class OrderRepository(OrderDbContext dbContext) : IOrder
{
    public async Task<Response> CreateAsync(Order entity)
    {
        try
        {
            var order = dbContext.Orders.Add(entity);
            await dbContext.SaveChangesAsync();
            return order.Entity.Id > 0 ? new Response(true, "Order Created") : new Response(false, "Error while Order Creation");
        }
        catch (Exception ex)
        {
            LogExceptions.LogException(ex);
            return new Response(false, "Failed to create order");
        }
    }

    public async Task<Response> UpdateAsync(Order entity)
    {
        try
        {
            var order = await GetByIdAsync(entity.Id);
            if(order is null) return new Response(false, "Order not found");
            dbContext.Entry(order).CurrentValues.SetValues(entity);
            await dbContext.SaveChangesAsync();
            return new Response(true, $"Order is updated successfully!");
        }
        catch (Exception ex)
        {
            LogExceptions.LogException(ex);
            return new Response(false, "Failed to create order");
        }
    }

    public async Task<Response> DeleteAsync(Order entity)
    {
        try
        {
            var order = await GetByIdAsync(entity.Id);
            if (order is null)
            {
                return new Response(false, "Order not found");
            }
            dbContext.Orders.Remove(order);
            await dbContext.SaveChangesAsync();
            return new Response(true, "Order Deleted");
        }
        catch (Exception ex)
        {
            LogExceptions.LogException(ex);
            return new Response(false, "Order not found");
        }
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        try
        {
           var orders = await dbContext.Orders.AsNoTracking().ToListAsync();
           return orders is not null ? orders : null!;

        }
        catch (Exception ex)
        {
            LogExceptions.LogException(ex);
            throw new Exception("Order not found");
        }
    }

    public async Task<Order> GetByIdAsync(int id)
    {
        try
        {
            var order = await dbContext.Orders.FindAsync(id);
            return order is not null ? order : null!;
        }
        catch (Exception ex)
        {
            LogExceptions.LogException(ex);
            throw new Exception("Order not found");
        }
    }

    public async Task<Order> GetByAsync(Expression<Func<Order, bool>> predicate)
    {
        try
        {
            var order = await dbContext.Orders.Where(predicate).FirstOrDefaultAsync();
            return order is not null ? order : null!;
        }
        catch (Exception ex)
        {
            LogExceptions.LogException(ex);
            throw new Exception("Order not found");
        }
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
    {
        try
        {
            var orders = await dbContext.Orders.Where(predicate).ToListAsync();
            return orders is not null ? orders : null!;
        }
        catch (Exception ex)
        {
            LogExceptions.LogException(ex);
            throw new Exception("Order not found");
        }
    }
}