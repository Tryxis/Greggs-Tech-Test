using System;
using System.Collections.Generic;
using System.Linq;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Interfaces;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Greggs.Products.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly IDataAccess<Product> _productAccess;
    private readonly IProductRepository _productRepository;

    // In a real world application would have this linked to an external API to uopdate dynamically without redeploying
    // But don't want to over-engineer this exercise
    private const decimal GBP_TO_EUR_EXCHANGE_RATE = 1.11m;

    public ProductController(
        ILogger<ProductController> logger,
        IDataAccess<Product> productAccess,
        IProductRepository productRepository)
    {
        _logger = logger;
        _productAccess = productAccess;
        _productRepository = productRepository;
    }


    // If accessing an external DB I would make these functions async
    // However kept them synchronous for the purpose of this exercise 
    [HttpGet]
    public IActionResult GetProducts(int pageStart = 0, int pageSize = 5)
    {
        if (pageStart < 0 || pageSize <= 0)
        {
            return BadRequest("Invalid pagination parameters.");
        }

        var products = _productAccess.List(pageStart, pageSize);

        if (!products.Any())
        {
            _logger.LogWarning("No products found for the given pagination range: Start={PageStart}, Size={PageSize}", pageStart, pageSize);
            return NotFound("No products available.");
        }

        return Ok(products);
    }

    [HttpGet("euro-prices")]
    public IActionResult GetProductsInEuros(int pageStart = 0, int pageSize = 5)
    {
        if (pageStart < 0 || pageSize <= 0)
        {
            return BadRequest("Invalid pagination parameters.");
        }

        var products = _productAccess.List(pageStart, pageSize);

        if (!products.Any())
        {
            _logger.LogWarning("No products found for the given pagination range: Start={PageStart}, Size={PageSize}", pageStart, pageSize);
            return NotFound("No products available.");
        }

        var productsWithEuroPrices = products.Select(p => new Product
        {
            Name = p.Name,
            PriceInPounds = p.PriceInPounds,
            PriceInEuros = _productRepository.ConvertToEuros(p.PriceInPounds, GBP_TO_EUR_EXCHANGE_RATE)
        });

        return Ok(productsWithEuroPrices);
    }
}
