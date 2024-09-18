using System.Linq.Expressions;
using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;

namespace ProductApi.Infrastructure.Repositories;

internal class ProductRepository(ProductDbContext dbContext) : IProduct
{
    public async Task<Response> CreateAsync(Product entity)
    {
        try
        {
            var product = await GetByAsync(p => p.Name!.Equals(entity.Name));
            if (product is not null && !string.IsNullOrEmpty(product.Name))
                return new Response(false, $"{entity.Name} already exists");
            dbContext.Products.Add(entity);
            await dbContext.SaveChangesAsync();
            return entity.Id > 0
                ? new Response(true, $"{entity.Name} added")
                : new Response(false, $"Error Occurred while adding {entity.Name}");
        }
        catch (Exception e)
        {
            LogExceptions.LogException(e);
            return new Response(false, "An error occured while adding product");
        }
    }

    public async Task<Response> UpdateAsync(Product entity)
    {
        try
        {
            var productToUpdate = await GetByIdAsync(entity.Id);
            if (productToUpdate is null)
                return new Response(false, $"{entity.Name} does not exists");
            dbContext.Entry(productToUpdate).CurrentValues.SetValues(entity);
            await dbContext.SaveChangesAsync();
            return new Response(true, $"{entity.Name} is updated successfully!");
        }
        catch (Exception e)
        {
            LogExceptions.LogException(e);
            return new Response(false, $"An error occured while updating {entity.Name}");
        }
    }

    public async Task<Response> DeleteAsync(Product entity)
    {
        try
        {
            var product = await GetByIdAsync(entity.Id);
            if (product is null)
                return new Response(false, $"{entity.Name} does not exists");
            var currentEntity = dbContext.Products.Remove(entity);
            await dbContext.SaveChangesAsync();
            return new Response(true, $"{entity.Name} is deleted successfully!");
        }
        catch (Exception e)
        {
            LogExceptions.LogException(e);
            return new Response(false, "An error occured while deleting product");
        }
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        try
        {
            var products = await dbContext.Products.AsNoTracking().ToListAsync();
            return products is not null ? products : null!;
        }
        catch (Exception e)
        {
            LogExceptions.LogException(e);
            throw new Exception("An error occured while retrieving products");
        }
    }
    public async Task<Product> GetByIdAsync(int id)
    {
        try
        {
            var product = await dbContext.Products.FindAsync(id);
            return product is not null ? product : null!;
        }
        catch (Exception e)
        {
            LogExceptions.LogException(e);
            throw new Exception("An error occured while retrieving product");
        }
    }
    public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
    {
        try
        {
            var productByPredicate = await dbContext.Products.Where(predicate).FirstOrDefaultAsync()!;
            return productByPredicate is not null ? productByPredicate : null!;
        }
        catch (Exception e)
        {
            LogExceptions.LogException(e);
            throw new Exception("An error occured while retrieving product");
        }
    }
}