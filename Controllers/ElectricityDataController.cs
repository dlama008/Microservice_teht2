using Microsoft.AspNetCore.Mvc;
using Microservice_Teht2.Data;
using Microservice_Teht2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Newtonsoft.Json;

namespace Microservice_Teht2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElectricityDataController : ControllerBase
    {
        private readonly ElectricityPriceDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public ElectricityDataController(ElectricityPriceDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> PostElectricityPrice([FromBody] ElectricityPriceInfo priceInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ElectricityPrices.Add(priceInfo);
            await _context.SaveChangesAsync();

            return Created();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetElectricityPrice(int id)
        {
            var priceInfo = await _context.ElectricityPrices.FindAsync(id);
            if (priceInfo == null)
            {
                return NotFound();
            }
            return Ok(priceInfo);
        }

        [HttpGet]
        public async Task<IActionResult> GetElectricityPrices()
        {
            var prices = await _context.ElectricityPrices.ToListAsync();
            return Ok(prices);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateElectricityPrice(int id, [FromBody] ElectricityPriceInfo priceInfo)
        {
            if (id != priceInfo.Id)
            {
                return BadRequest();
            }

            _context.Entry(priceInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ElectricityPrices.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteElectricityPrice(int id)
        {
            var priceInfo = await _context.ElectricityPrices.FindAsync(id);
            if (priceInfo == null)
            {
                return NotFound();
            }

            _context.ElectricityPrices.Remove(priceInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/ElectricityData/range
        [HttpGet("range")]
        public IActionResult GetPricesByDateRange(DateTime startDate, DateTime endDate)
        {
            var prices = _context.ElectricityPrices
                                 .Where(p => p.StartDate >= startDate && p.EndDate <= endDate)
                                 .ToList();
            if (!prices.Any())
            {
                return NotFound("Ei löytynyt hintatietoja annetulta aikaväliltä.");
            }

            return Ok(prices);
        }

        // Lisäys: Hae dataa toiselta mikropalvelulta ja tallenna se tietokantaan
        [HttpPost("fetch-and-store")]
        public async Task<IActionResult> FetchAndStoreData()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync("https://localhost:7034/api/ElectricityData"); // Osoita oikea endpoint
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var pricesContainer = JsonConvert.DeserializeObject<List<ElectricityPriceInfo>>(content);

                if (pricesContainer != null)
                {
                    foreach (var priceInfo in pricesContainer)
                    {
                        _context.ElectricityPrices.Add(priceInfo);
                    }
                    await _context.SaveChangesAsync();
                    return Ok("Data fetched and stored successfully.");
                }

                return NotFound("Data not found or unable to deserialize.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
