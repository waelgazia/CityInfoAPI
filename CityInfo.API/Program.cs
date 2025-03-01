using System.Diagnostics;
using CityInfo.API.Data;
using CityInfo.API.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.OpenApi.Models;

namespace CityInfo.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers().AddNewtonsoftJson();
        
        // to return a formatted message in exception.
        builder.Services.AddProblemDetails(); 
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.AddSingleton<CitiesDataStore>();
        builder.Services.AddDbContext<AppDbContext>();
        builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();
        
        builder.Services.AddTransient<IMailService, LocalMailService>();
        builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Authentication:Issuer"],
                ValidAudience = builder.Configuration["Authentication:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Convert.FromBase64String(builder.Configuration["Authentication:SecretForKey"]))
            };
        });
        
        builder.Services.AddAuthorization(options => 
        {
            options.AddPolicy("MustBeFromParis", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("city", "Paris");
            });
        });
        
        builder.Services.AddApiVersioning(setupAction => 
        {
            setupAction.ReportApiVersions = true;
            setupAction.AssumeDefaultVersionWhenUnspecified = true;
            setupAction.DefaultApiVersion = new Asp.Versioning.ApiVersion(1,0);
        }).AddMvc();
        
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

        app.UseAuthentication(); /* important for JWT authentication */
        
        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}