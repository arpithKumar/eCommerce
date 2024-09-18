using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class ProductsController(IProduct productInterface): ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await productInterface.GetAllAsync();
        if (!products.Any()) return NotFound("No Products found!");
        var (_, list) = ProductConversion.FromEntity(null, products);
        return list.Any() ? Ok(list) : NotFound("No Product Found");
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await productInterface.GetByIdAsync(id);
        if (product is null) return NotFound("No Products found!");
        var (p, _) = ProductConversion.FromEntity(product, null);
        return p is not null ? Ok(p) : NotFound("No Product Found"); 
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Response>> CreateProduct(ProductDto product)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var entity = ProductConversion.ToEntity(product);
        var response = await productInterface.CreateAsync(entity);
        return response.Flag ? Ok(response) : BadRequest(response);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Response>> UpdateProduct(ProductDto product)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var entity = ProductConversion.ToEntity(product);
        var response = await productInterface.UpdateAsync(entity);
        return response.Flag ? Ok(response) : BadRequest(response); 
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Response>> DeleteProduct(ProductDto product)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var entity = ProductConversion.ToEntity(product);
        var response = await productInterface.DeleteAsync(entity);
        return response.Flag ? Ok(response) : BadRequest(response);
    }
}