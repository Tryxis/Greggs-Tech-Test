using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Greggs.Products.Api.Interfaces
{
    public interface IProductRepository
    {
        public decimal ConvertToEuros(decimal amountInPounds, decimal exchangeRate);
    }
}