using CityInfo.API.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CityInfo.API.ActionFilters;

public class CityExistsFilterAttribute : IActionFilter
{
    private readonly IUnitOfWork _unitOfWork;
    public CityExistsFilterAttribute(IUnitOfWork unitOfWork) =>
        _unitOfWork = unitOfWork;

    public void OnActionExecuted(ActionExecutedContext context) { }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var cityId = (int)context.ActionArguments["cityId"]!;
        var city = _unitOfWork.CityRepository.GetCityAsync(cityId, true).Result;
        if (city == null)
            context.Result = new NotFoundResult();
    }
}
