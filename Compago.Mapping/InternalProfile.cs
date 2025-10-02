using AutoMapper;
using Compago.Data;
using Compago.Domain;

namespace Compago.Mapping
{
    public class InternalProfile : Profile
    {
        public InternalProfile()
        {
            CreateMap<User, UserDTO>()
                .ForMember(to => to.RoleName, c => c.MapFrom(from => from.Role.Name))
                .ForMember(to => to.RoleId, c => c.MapFrom(from => (Common.Role)from.RoleId))
                .ReverseMap()
                .ForMember(to => to.RoleId, c => c.MapFrom(from => (short)from.RoleId))
                .ForMember(to => to.Role, opt => opt.Ignore());

            CreateMap<User, UserSecurityCredentialsDTO>()
                .ForMember(to => to.RoleId, c => c.MapFrom(from => (Common.Role)from.RoleId));

            CreateMap<Tag, TagDTO>().ReverseMap();

            CreateMap<InvoiceTag, InvoiceTagDTO>().ReverseMap();
        }
    }
}
