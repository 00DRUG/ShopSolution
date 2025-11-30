using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shop.Application.DTOs;
using Shop.Application.Services;
using Shop.Application.Validators;
using Shop.Domain.Interfaces;
using Shop.Infrastructure.BackgroundJobs;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Repositories;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=shop.db";
builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseSqlite(connectionString));
//register layers(DI)
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

//register validators
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IValidator<CreateProductDto>, CreateProductValidator>();

//V2 CONFIG 
//API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0); // default to v1.0
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; 
})
.AddMvc()
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
//Background Worker for Stock Updates
builder.Services.AddSingleton<IStockQueue, StockQueue>();
builder.Services.AddHostedService<StockUpdateWorker>();
//Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Shop API V1",
        Version = "v1",
        Description = "Standard synchronous API."
    });

    c.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "Shop API V2",
        Version = "v2",
        Description = "Async API with Background Queue and Pagination."
    });
});

var app = builder.Build();

// ensure db created or connected
using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ShopDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shop API V1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Shop API V2");
    }); // for the swagger versions switcher
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();