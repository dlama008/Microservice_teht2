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
                    // Tarkista, onko tietokannassa jo tietuetta samalla p‰iv‰m‰‰r‰ll‰
                    var isDuplicate = await _electricityDbContext.ElectricityPriceInfos.AnyAsync(x =>
                        x.StartDate == hourPrice.StartDate && x.EndDate == hourPrice.EndDate);

                    if (!isDuplicate)
                    {
                        // Jos samalla p‰iv‰m‰‰r‰ll‰ ei ole tietuetta, lis‰‰ uusi tietue
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

            return Ok("Data vastaanotettu ja k‰sitelty.");
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

    }
}
