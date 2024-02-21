using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microservice_Teht2.Data;

var builder = WebApplication.CreateBuilder(args);

// Lis‰‰ palvelut konteineriin.
builder.Services.AddHttpClient();
builder.Services.AddDbContext<ElectricityPriceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Electricity Data API", Version = "v1" });
});

// CORS-konfiguraatio, sallii kaikki alkuper‰t, metodit ja otsikot.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
        policyBuilder.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader());
});

var app = builder.Build();

// K‰yt‰ CORS-middlewarea ennen UseRouting ja muita middlewareja.
app.UseCors("AllowAll");

// Konfiguroi HTTP-pyyntˆputki.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Electricity Data API v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
