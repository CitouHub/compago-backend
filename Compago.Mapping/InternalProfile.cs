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

            CreateMap<InvoiceTag, InvoiceTagDTO>()
                .ForMember(to => to.TagName, c => c.MapFrom(from => from.Tag.Name))
                .ForMember(to => to.TagColor, c => c.MapFrom(from => from.Tag.Color))
                .ReverseMap()
                .ForMember(to => to.Tag, opt => opt.Ignore());
        }
    }
}
