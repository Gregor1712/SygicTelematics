using AutoMapper;
using Catalog.Application.DTOs;
using Catalog.Application.Entities;

namespace Catalog.Application.Mapper;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<CpuDbo, CpuDto>();
        CreateMap<CpuDto, CpuDbo>();

        CreateMap<ManufacturerDbo, ManufacturerDto>();
        CreateMap<ManufacturerDto, ManufacturerDbo>();
    }
}