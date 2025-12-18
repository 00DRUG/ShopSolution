using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shop.Application.DTOs;
using Shop.Application.Interfaces;
using Shop.Application.Services;
using Shop.Application.Validators;
using Shop.Domain.Entities;
using Shop.Domain.Interfaces;
using Shop.Infrastructure.BackgroundJobs;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Repositories;
using Shop.Infrastructure.Services;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=Data/shop.db";
builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseSqlite(connectionString));

//register layers(DI)
builder.Services
    .AddScoped<IProductRepository, ProductRepository>()
    .AddScoped<IProductService, ProductService>();

//register validators
builder.Services
    .AddFluentValidationAutoValidation()
    .AddScoped<IValidator<CreateProductDto>, CreateProductValidator>();

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
builder.Services
    .AddSingleton<IStockQueue, StockQueue>()
    .AddHostedService<StockUpdateWorker>();

//Swagger configuration
builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen(c =>
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

//AutoMapper configuration
builder.Services.AddAutoMapper(config =>
{
    config.AddMaps(typeof(Shop.Application.Mappings.ProductProfile).Assembly);
});

// Authentication
builder.Services.AddScoped<ITokenService, TokenService>();

//Set identity
builder.Services.AddIdentityCore<AppUser>(opt => {
    opt.Password.RequireNonAlphanumeric = false; // easy password for demo
})
.AddEntityFrameworkStores<ShopDbContext>()
.AddSignInManager<SignInManager<AppUser>>();

// JWT Authentication configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:Key"]!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Ensure db created or connected
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ShopDbContext>();

    // Get the path where the app is running
    var executionPath = AppDomain.CurrentDomain.BaseDirectory;

    //Create Data directory if not exists
    var dataPath = Path.Combine(executionPath, "Data");

    if (!Directory.Exists(dataPath))
    {
        Directory.CreateDirectory(dataPath);
    }

    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app
        .UseSwagger()
        .UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shop API V1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Shop API V2");
    }); // for the swagger versions switcher
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();