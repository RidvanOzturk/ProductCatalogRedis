using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProductCatalogRedis.Caching;
using ProductCatalogRedis.Data;
using ProductCatalogRedis.Services.Contracts;
using ProductCatalogRedis.Services.Implementations;
using StackExchange.Redis;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


var sqlConn = builder.Configuration.GetConnectionString("DefaultConnection");
var redisConn = builder.Configuration.GetConnectionString("Redis")!;
builder.Services.Configure<CacheSettings>(
    builder.Configuration.GetSection("CacheSettings"));
builder.Services.AddDbContextPool<ProductContext>(opt =>
    opt.UseSqlServer(sqlConn));

builder.Services.AddStackExchangeRedisCache(opts => {
    opts.Configuration = builder.Configuration.GetConnectionString("Redis");
    opts.InstanceName = "productcache:";
});

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
