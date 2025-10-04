using AutoMapper;
using Compago.Domain;
using Compago.Domain.ExternalSourceExample.MicosoftAzure;
using System.Globalization;

namespace Compago.Mapping
{
    public class MicrosoftAzureProfile : Profile
    {
        public MicrosoftAzureProfile()
        {
            CreateMap<Monthly, InvoiceDTO>()
                .ForMember(to => to.Price, c => c.MapFrom(from => double.Parse(from.Bill.MoneyToPay.Replace(",", "."), CultureInfo.InvariantCulture)))
                .ForMember(to => to.Id, c => c.MapFrom(from => from.Bill.Reference.ToString()))
                .ForMember(to => to.Date, c => c.MapFrom(from => from.IssueDate));
        }
    }
}
