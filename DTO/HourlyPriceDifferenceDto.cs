
using System;

namespace Microservice_teht2.DTO
{
    public class HourlyPriceDifferenceDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal MarketPrice { get; set; }
        public decimal FixedPrice { get; set; }
        public decimal Difference { get; set; }
    }
}
