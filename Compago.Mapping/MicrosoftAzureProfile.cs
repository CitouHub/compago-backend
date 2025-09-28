using AutoMapper;
using Compago.Domain;
using Compago.Domain.ExternalSourceExample.MicosoftAzure;

namespace Compago.Mapping
{
    public class MicrosoftAzureProfile : Profile
    {
        public MicrosoftAzureProfile()
        {
            CreateMap<Expenses, BillingDTO>()
                .ForMember(to => to.Currency, c => c.MapFrom(from => from.Currency.ToUpper()))
                .ForMember(to => to.Invoices, c => c.MapFrom(from => from.Monthly));

            CreateMap<Monthly, InvoiceDTO>()
                .ForMember(to => to.Price, c => c.MapFrom(from => from.Bill.MoneyToPay))
                .ForMember(to => to.Id, c => c.MapFrom(from => from.Bill.Reference))
                .ForMember(to => to.Date, c => c.MapFrom(from => from.IssueDate));
        }
    }
}
