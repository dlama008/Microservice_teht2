using Microsoft.AspNetCore.Mvc;
using Microservice_Teht2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microservice_teht2.DTO;
using Microservice_teht2.Extensions;
using Microservice_teht2.Models;
using Microservice_teht2;

namespace Microservice_Teht2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class ElectricityDataController : ControllerBase
    {
        private ElectricityPriceDbContext _electricityDbContext;
        private ILogger<ElectricityDataController> _logger;
        private readonly HttpClient _httpClient;

        public ElectricityDataController(ElectricityPriceDbContext electricityDbContext, ILogger<ElectricityDataController> logger, IHttpClientFactory httpClientFactory)
        {
            _electricityDbContext = electricityDbContext;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ElectricityPriceDataDtoIn data)
        {
            if (data == null)
            {
                return BadRequest("Dataa ei vastaanotettu.");
            }

            try
            {
                foreach (var hourPrice in data.Prices)
                {
                    // Tarkista, onko tietokannassa jo tietuetta samalla päivämäärällä
                    var isDuplicate = await _electricityDbContext.ElectricityPriceInfos.AnyAsync(x =>
                        x.StartDate == hourPrice.StartDate && x.EndDate == hourPrice.EndDate);

                    if (!isDuplicate)
                    {
                        // Jos samalla päivämäärällä ei ole tietuetta, lisää uusi tietue
                        _electricityDbContext.ElectricityPriceInfos.Add(hourPrice.ToEntity());
                    }
                    else
                    {
                        _logger.LogInformation($"Duplikaatti havaittu, ei tallenneta uudestaan: {hourPrice.StartDate} - {hourPrice.EndDate}");
                    }
                }

                await _electricityDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Virhe tallennettaessa dataa tietokantaan: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Virhe tallennettaessa dataa tietokantaan.");
            }

            return Ok("Data vastaanotettu ja käsitelty.");
        }

        //POISTA
        [HttpDelete("DeleteByDate")]
        public async Task<IActionResult> DeleteByDate(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Etsi poistettavat tietueet
                var recordsToDelete = await _electricityDbContext.ElectricityPriceInfos.Where(x => x.StartDate >= startDate && x.EndDate <= endDate).ToListAsync();

                // Poista tietueet
                _electricityDbContext.ElectricityPriceInfos.RemoveRange(recordsToDelete);
                await _electricityDbContext.SaveChangesAsync();

                return Ok("Data poistettu tietokannasta.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Virhe poistettaessa dataa tietokannasta: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Virhe poistettaessa dataa tietokannasta.");
            }
        }
        //HINTA AIKAVÄLILLÄ

        [HttpGet("GetElectricityPricesFromRange")]
        public async Task<IActionResult> GetElectricityPricesFromRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int pageSize = 10, [FromQuery] int page = 1)
        {
            if (startDate >= endDate)
            {
                return BadRequest("Start date must be before end date.");
            }

            try
            {
                int skip = (page - 1) * pageSize;
                var electricityPrices = await _electricityDbContext.ElectricityPriceInfos
                .Where(x => x.StartDate >= startDate && x.StartDate < endDate.AddDays(1))
                .OrderBy(x => x.StartDate)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();


                return Ok(electricityPrices);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching data from database: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while fetching the electricity prices.");
            }
        }
        //HINTAERO AIKAVÄLILLÄ

        [HttpGet("GetPriceDifferenceFromRange")]
        public async Task<IActionResult> GetPriceDifferenceFromRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] decimal fixedPrice)
        {
            if (startDate >= endDate)
            {
                return BadRequest("Start date must be before end date.");
            }

            try
            {
                var electricityPrices = await _electricityDbContext.ElectricityPriceInfos
                    .Where(x => x.StartDate >= startDate && x.EndDate <= endDate)
                    .OrderBy(x => x.StartDate)
                    .ToListAsync();

                var priceDifferences = electricityPrices.Select(priceInfo =>
                    new PriceDifferenceDto
                    {
                        StartDate = priceInfo.StartDate,
                        EndDate = priceInfo.EndDate,
                        PriceDifferenceValue = Math.Round(priceInfo.Price - fixedPrice, 3),
                        CheaperContract = priceInfo.Price > fixedPrice ? ElectricityContractType.MarketPrice : ElectricityContractType.FixedPrice
                    }).ToList();

                return Ok(priceDifferences);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calculating price differences: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while calculating the price differences.");
            }
        }

        //BONARI
        //TUNNEITTAIN HINTAERO
        [HttpGet("GetHourlyPriceDifferences")]
        public async Task<IActionResult> GetHourlyPriceDifferences([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] decimal fixedPrice)
        {
            if (startDate >= endDate)
            {
                return BadRequest("Start date must be before end date.");
            }

            try
            {
                var hourlyPrices = await _electricityDbContext.ElectricityPriceInfos
                    .Where(x => x.StartDate >= startDate && x.EndDate <= endDate)
                    .OrderBy(x => x.StartDate)
                    .ToListAsync();

                var hourlyDifferences = hourlyPrices
                    .Select(hourPrice => new HourlyPriceDifferenceDto
                    {
                        StartDate = hourPrice.StartDate,
                        EndDate = hourPrice.EndDate,
                        MarketPrice = hourPrice.Price,
                        FixedPrice = fixedPrice,
                        Difference = Math.Round(hourPrice.Price - fixedPrice, 3)
                    })
                    .ToList();

                return Ok(hourlyDifferences);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calculating hourly price differences: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occurred while calculating the hourly price differences.");
            }
        }

    }
}
