using Microsoft.EntityFrameworkCore;
using SmartInvoice.Invoice.Data;
using SmartInvoice.Invoice.Services;

var builder = WebApplication.CreateBuilder(args);

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:5173") // React/Vite dev servers
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials()
                  .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
        });

    // Add a more permissive policy for development/testing
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "SmartInvoice Invoice API", Version = "v1" });
});

// Configure DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    // Fallback to a default SQLite connection if not configured
    connectionString = "Data Source=invoices.db";
}

builder.Services.AddDbContext<InvoiceDbContext>(options =>
    options.UseSqlite(connectionString));

// Register services
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartInvoice Invoice API v1");
        c.RoutePrefix = "swagger"; // Access at /swagger
    });

    // Use permissive CORS in development
    app.UseCors("AllowFrontend");

    Console.WriteLine("Running in Development mode");
}
else
{
    // Production settings
    app.UseHttpsRedirection();
    app.UseCors("AllowFrontend");

    Console.WriteLine("Running in Production mode");
}

// Use CORS middleware (must be before UseAuthorization and MapControllers)
app.UseCors("AllowFrontend");

app.UseAuthorization();
app.MapControllers();

// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<InvoiceDbContext>();

        // This will create the database if it doesn't exist and apply any pending migrations
        await dbContext.Database.EnsureCreatedAsync();

        // Or use migrations if you have them:
        // await dbContext.Database.MigrateAsync();

        Console.WriteLine("Database created/verified successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while creating the database: {ex.Message}");
    }
}

app.Run();