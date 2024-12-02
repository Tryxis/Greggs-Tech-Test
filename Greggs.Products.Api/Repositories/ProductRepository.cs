using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Greggs.Products.Api.Interfaces;

namespace Greggs.Products.Api.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public decimal ConvertToEuros(decimal amountInPounds, decimal exchangeRate)
        {
            return Math.Round(amountInPounds * exchangeRate, 2);
        }
    }
}

// In a wider application this would likely be a service.
// This is here to demonstrate repository model and interface DI, abstracting things away from the Controller.