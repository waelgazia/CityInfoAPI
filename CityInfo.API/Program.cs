using System.Diagnostics;
using CityInfo.API.Data;
using CityInfo.API.Services;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers(options =>
        {
            // returns 406 Not Acceptable error if the client requests for other format than JSON (default).
            options.ReturnHttpNotAcceptable = true;
        }).AddNewtonsoftJson();
        
        // to return a formatted message in exception.
        builder.Services.AddProblemDetails(); 
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.AddSingleton<CitiesDataStore>();
        builder.Services.AddDbContext<AppDbContext>();
        
        builder.Services.AddTransient<IMailService, LocalMailService>();
        builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

        var app = builder.Build();

        // To globally handle exception in production,
        // and return a formatted message.
        if (!app.Environment.IsDevelopment() /* PRODUCTION */)
        {
            app.UseExceptionHandler();
        }
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}