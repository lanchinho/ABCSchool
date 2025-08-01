using Application;
using Infrastructure;
using System.Text.Json.Serialization;
using WebApi.Middlewares;

namespace WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("ABC School App", policy =>
            {
                policy.WithOrigins(["https://localhost:7089", "http://localhost:5248"])
                    .AllowAnyHeader()
                    .AllowAnyMethod();                    
            });
        });

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.WriteIndented = true;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddJwtAuthentication(builder.Services.GetJwtSettings(builder.Configuration));

        builder.Services.AddApplicationServices();
        var app = builder.Build();

        //Database Seeder
        await app.Services.AddDatabaseInitializerAsync();

        app.UseHttpsRedirection();

        app.UseCors("ABC School App");
        app.UseInfraStructure();

        app.UseMiddleware<ErrorHandlingMiddleware>();

        app.MapControllers();

        app.Run();
    }
}
