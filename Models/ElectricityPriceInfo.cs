using System;
using System.ComponentModel.DataAnnotations;

namespace Microservice_Teht2.Models
{
    public class ElectricityPriceInfo
    {
        [Key]
        public int Id { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
