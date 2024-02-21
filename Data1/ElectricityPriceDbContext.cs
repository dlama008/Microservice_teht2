using System;
using Microsoft.EntityFrameworkCore;
using Microservice_Teht2.Models;

namespace Microservice_Teht2.Data
{
    public class ElectricityPriceDbContext : DbContext
    {
        public ElectricityPriceDbContext(DbContextOptions<ElectricityPriceDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }

        public DbSet<ElectricityPriceInfo> ElectricityPrices { get; set; }
    }
}