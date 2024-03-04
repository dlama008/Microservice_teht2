using System.Runtime.CompilerServices;

namespace Microservice_teht2.DTO
{
    public class ElectricityPriceDataDtoIn
    {
        public List<PriceInfo> Prices { get; set; }

    }

    public class PriceInfo
    {
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

}
