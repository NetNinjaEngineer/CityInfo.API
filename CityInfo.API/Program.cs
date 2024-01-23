
using CityInfo.API.Data;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace CityInfo.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers(options =>
        {
            options.ReturnHttpNotAcceptable = true;
        })
            .AddXmlDataContractSerializerFormatters()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAutoMapper(typeof(Program).Assembly);
        builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
        var connectionString = builder.Configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

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