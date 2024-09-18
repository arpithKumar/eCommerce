using ProductApi.Domain.Entities;

namespace ProductApi.Application.DTOs.Conversions;

public static class ProductConversion
{
    public static Product ToEntity(this ProductDto productDto) => new()
    {
        Id = productDto.Id,
        Name = productDto.Name,
        Price = productDto.Price,
        Quantity = productDto.Quantity
    };

    public static (ProductDto?, IEnumerable<ProductDto>?) FromEntity(Product? product,
        IEnumerable<Product>? products)
    {
        if (product is not null || products is null)
        {
            return (new ProductDto(
                product!.Id,
                product.Name!,
                product.Quantity,
                product.Price
            ), null);
        }

        if (products is not null || product is null)
        {
            return (null, products!.Select(prod => new ProductDto( prod.Id, prod.Name!, prod.Quantity, prod.Price)).ToList());
        }
        
        return (null, null);
    }
}