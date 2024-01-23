﻿using AutoMapper;
using CityInfo.API.DataTransferObjects;
using CityInfo.API.Models;

namespace CityInfo.API.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<PointOfInterest, PointOfInterestDto>().ReverseMap();
        CreateMap<PointOfInterest, PointOfInterestForUpdateDto>().ReverseMap();
        CreateMap<PointOfInterest, PointOfInterestForCreationDto>().ReverseMap();
        CreateMap<City, CityForCreationDto>().ReverseMap();
    }
}
