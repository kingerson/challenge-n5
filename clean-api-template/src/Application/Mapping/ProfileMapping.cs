namespace MsClean.Application;
using AutoMapper;
using MsClean.Domain;

public class ProfileMapping : Profile
{
    public ProfileMapping() => CreateMap<Permission, PermissionViewModel>()
               .ForMember(t => t.Id, o => o.MapFrom(t => t.Id))
               .ForMember(t => t.EmployeeForename, o => o.MapFrom(t => t.EmployeeForename))
               .ForMember(t => t.EmployeeLastName, o => o.MapFrom(t => t.EmployeeLastName))
               .ForMember(t => t.PermissionDate, o => o.MapFrom(t => t.PermissionDate))
               .ForMember(t => t.UserRegister, o => o.MapFrom(t => t.UserRegister))
               .ForMember(t => t.DateTimeRegister, o => o.MapFrom(t => t.DateTimeRegister))
               .ForMember(t => t.PermissionTypeId, o => o.MapFrom(t => t.PermissionTypeId))
               .ForMember(t => t.IsActive, o => o.MapFrom(t => t.IsActive))
               .ReverseMap();
}
