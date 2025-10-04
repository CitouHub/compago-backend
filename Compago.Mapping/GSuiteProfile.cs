using AutoMapper;
using Compago.Domain;
using Compago.Domain.ExternalSourceExample.GSuite;

namespace Compago.Mapping
{
    public class GSuiteProfile : Profile
    {
        public GSuiteProfile()
        {
            CreateMap<InvoiceDescription, InvoiceDTO>()
                .ForMember(to => to.Price, c => c.MapFrom(from => from.Cost))
                .ForMember(to => to.Date, c => c.MapFrom(from => from.InvoiceDate));
        }
    }
}
