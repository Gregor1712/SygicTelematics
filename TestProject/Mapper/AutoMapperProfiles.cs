using AutoMapper;
using TestProject.Dbo;
using TestProject.DTO;

namespace TestProject.Mapper;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<CpuDBO, CpuDTO>();
        CreateMap<CpuDTO, CpuDBO>();

        CreateMap<ManufacturerDBO, ManufacturerDTO>();
        CreateMap<ManufacturerDTO, ManufacturerDBO>();

        // CreateMap<RoleDBO, RoleDto>();
        // CreateMap<RoleDto, RoleDBO>();

        //.ForMember(x => x.Role, opt => opt.);

        //.ForMember(m => m.Role, opt => opt.Ignore())
        //.ForAllMembers(x => x.Role.Users, opt => opt.Ignore());
        //CreateMap<UserDto, UserDBO>();

        //Mapper.Map<OrderLine, OrderLineDTO>()
        //    .ForMember(m => m.Order, opt => opt.Ignore());
    }
}