using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add memory cache
builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowClient");

// ✅ Inject IMemoryCache into endpoint
app.MapGet("/api/products", (IMemoryCache cache) =>
{
    const string cacheKey = "products";

    if (!cache.TryGetValue(cacheKey, out object? cachedData))
    {
        var data = new[]
        {
            new { Id = 1, Name = "Laptop", Price = 1200.50, Stock = 25, Category = new { Id = 101, Name = "Electronics" } },
            new { Id = 2, Name = "Smartphone", Price = 799.99, Stock = 50, Category = new { Id = 101, Name = "Electronics" } },
            new { Id = 3, Name = "Headphones", Price = 199.99, Stock = 100, Category = new { Id = 101, Name = "Electronics" } },
            new { Id = 4, Name = "Office Chair", Price = 150.00, Stock = 15, Category = new { Id = 102, Name = "Furniture" } },
            new { Id = 5, Name = "Desk Lamp", Price = 45.75, Stock = 40, Category = new { Id = 102, Name = "Furniture" } },
            new { Id = 6, Name = "Coffee Maker", Price = 89.99, Stock = 30, Category = new { Id = 103, Name = "Home Appliances" } },
            new { Id = 7, Name = "Blender", Price = 65.49, Stock = 20, Category = new { Id = 103, Name = "Home Appliances" } },
            new { Id = 8, Name = "Notebook", Price = 5.99, Stock = 200, Category = new { Id = 104, Name = "Stationery" } },
            new { Id = 9, Name = "Pen Set", Price = 12.49, Stock = 150, Category = new { Id = 104, Name = "Stationery" } },
            new { Id = 10, Name = "Backpack", Price = 39.99, Stock = 60, Category = new { Id = 105, Name = "Accessories" } }
        };

        var options = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))   // hard expiry
            .SetSlidingExpiration(TimeSpan.FromMinutes(2));   // refresh on access

        cache.Set(cacheKey, data, options);

        Console.WriteLine("✅ Data loaded and cached");

        return data;
    }

    Console.WriteLine("⚡ Returned from cache");

    return cachedData;
});

app.Run();