using System;

namespace Microservice_teht2.DTO
{
    public class PriceInfoDto
    {
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}