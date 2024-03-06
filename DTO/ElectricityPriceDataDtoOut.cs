namespace Microservice_teht2.DTO;
using System.Collections.Generic;

    public class ElectricityPriceDataDtoOut
    {
        public List<PriceInfo> Prices { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

