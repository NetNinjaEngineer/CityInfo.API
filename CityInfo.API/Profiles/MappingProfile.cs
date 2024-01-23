using AutoMapper;
using CityInfo.API.DataTransferObjects;
using CityInfo.API.Models;

namespace CityInfo.API.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<City, CityForCreationDto>().ReverseMap();
        CreateMap<City, CityForUpdateDto>().ReverseMap();
        CreateMap<PointOfInterest, PointOfInterestDto>().ReverseMap();
        CreateMap<PointOfInterest, PointOfInterestForUpdateDto>().ReverseMap();
        CreateMap<CityForUpdateDto, City>();

    }
}
