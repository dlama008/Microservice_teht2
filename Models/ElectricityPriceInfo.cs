using Microservice_teht2.Models;
using System;
using System.Collections.Generic; // Include this for List
using System.ComponentModel.DataAnnotations;

namespace Microservice_Teht2.Models
{
    public class ElectricityPriceInfo : BaseEntity
    {

        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }
        public decimal Price { get; set; }
    }
}
