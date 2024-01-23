using AutoMapper;
using CityInfo.API.DataTransferObjects;
using CityInfo.API.Models;

namespace CityInfo.API.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<PointOfInterest, PointOfInterestForUpdateDto>().ReverseMap();
    }
}
