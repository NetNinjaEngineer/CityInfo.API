using CityInfo.API.ActionFilters;
using CityInfo.API.Contracts;
using CityInfo.API.Data;
using CityInfo.API.Repository;
using CityInfo.API.Repository.Implementors;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json.Serialization;

namespace CityInfo.API;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog();

        builder.Services.AddControllers(options =>
        {
            options.ReturnHttpNotAcceptable = true;
        })
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
            .AddXmlDataContractSerializerFormatters()
            .ConfigureApiBehaviorOptions(setupAction =>
            {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetailsFactory = context.HttpContext.RequestServices
                    .GetRequiredService<ProblemDetailsFactory>();

                    var validationProblemDetails = problemDetailsFactory.CreateValidationProblemDetails(context.HttpContext,
                        context.ModelState);

                    validationProblemDetails.Detail = "See the errors field for details";
                    validationProblemDetails.Instance = context.HttpContext.Request.Path;

                    var actionExecutingContext = context as ActionExecutingContext;
                    if ((context.ModelState.ErrorCount > 0)
                        && (actionExecutingContext?.ActionArguments.Count ==
                        context.ActionDescriptor.Parameters.Count))
                    {
                        validationProblemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                        validationProblemDetails.Title = "One or more validation errors occurred";

                        return new UnprocessableEntityObjectResult(validationProblemDetails)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    }

                    validationProblemDetails.Status = StatusCodes.Status400BadRequest;
                    validationProblemDetails.Title = "One or more error on input occurred";
                    return new BadRequestObjectResult(validationProblemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });


        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
        var connectionString = builder.Configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddAutoMapper(typeof(Program).Assembly);
        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        builder.Services.AddScoped<ICityRepository, CityRepository>();
        builder.Services.AddScoped<IPointOfInterestRepository, PointOfInterestRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<CityExistsFilterAttribute>();
        builder.Services.AddScoped<ValidationFilterAttribute>();
        builder.Services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("An Unexpected fault happened, try again later.");
                });
            });

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
