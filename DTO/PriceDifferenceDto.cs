// Tiedosto: DTO/PriceDifferenceDto.cs

using System;

namespace Microservice_teht2.DTO
{
    public enum ElectricityContractType
    {
        FixedPrice,
        MarketPrice
    }

    public class PriceDifferenceDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PriceDifferenceValue { get; set; }
        public ElectricityContractType CheaperContract { get; set; }
    }
}
