using System.Diagnostics;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo;

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
  
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

        var app = builder.Build();
        
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