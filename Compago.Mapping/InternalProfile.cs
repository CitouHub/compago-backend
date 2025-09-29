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
                .ReverseMap()
                .ForMember(to => to.Role, opt => opt.Ignore());

            CreateMap<Tag, TagDTO>().ReverseMap();

            CreateMap<InvoiceTag, InvoiceTagDTO>().ReverseMap();
        }
    }
}
