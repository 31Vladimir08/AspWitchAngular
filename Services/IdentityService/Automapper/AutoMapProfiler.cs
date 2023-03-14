using AutoMapper;

using IdentityService.ModelDto;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Automapper
{
    public class AutoMapProfiler : Profile
    {
        public AutoMapProfiler()
        {
            CreateProfileDto();
        }
        
        private void CreateProfileDto()
        {
            CreateMap<RoleDto, IdentityRole>()
                .ForMember(x => x.NormalizedName, y => y.Ignore())
                .ForMember(x => x.ConcurrencyStamp, y => y.Ignore())
                .ForMember(x => x.Name, y => y.MapFrom(p => p.RoleName))
                .ForMember(x => x.NormalizedName, y => y.MapFrom(p => p.RoleCode));
            CreateMap<IdentityRole, RoleDto>()
                .ForMember(x => x.RoleName, y => y.MapFrom(p => p.Name))
                .ForMember(x => x.RoleCode, y => y.MapFrom(p => p.NormalizedName));
        }
    }
}
