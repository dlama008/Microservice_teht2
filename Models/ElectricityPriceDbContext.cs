using System;
using Microsoft.EntityFrameworkCore;
using Microservice_Teht2.Models;

namespace Microservice_teht2.Models
{
    public class ElectricityPriceDbContext : DbContext
    {
        public ElectricityPriceDbContext(DbContextOptions<ElectricityPriceDbContext> options)
        : base(options)
        {
        }
        public DbSet<ElectricityPriceInfo> ElectricityPriceInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\mssqllocaldb;Initial Catalog=ElectricityPriceDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is BaseEntity
                && (x.State == EntityState.Modified));

            var now = DateTime.Now;

            foreach (var entity in entities)
            {
                var baseEntity = (BaseEntity)entity.Entity;

                baseEntity.UpdatedAt = now;
            }
        }

    }

}