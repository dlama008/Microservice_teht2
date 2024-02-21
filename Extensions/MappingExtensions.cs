using Microservice_teht2;
using Microservice_teht2.DTO;
using Microservice_Teht2.Models;

namespace Microservice_teht2.Extensions
{
    public static class MappingExtensions
    {
        public static ElectricityPriceInfo ToEntity(this PriceInfo priceInfo)
        {
            return new ElectricityPriceInfo
            {
                StartDate = priceInfo.StartDate,
                EndDate = priceInfo.EndDate,
                Price = priceInfo.Price
            };
        }
    }
}