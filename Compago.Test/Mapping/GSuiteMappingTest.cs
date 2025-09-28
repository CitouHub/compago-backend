using AutoMapper;
using Compago.Domain;
using Compago.Test.Helper;
using Compago.Test.Helper.Domain.ExternalSourceExample;

namespace Compago.Test.Mapping
{
    public class GSuiteMappingTest
    {
        private static readonly IMapper _mapper = MapperHelper.DefineMapper();

        public class FinancialInfoMapping
        {
            [Theory]
            [InlineData("sek")]
            [InlineData("SeK")]
            [InlineData("SEK")]
            public void ToBillingDTO(string curreny)
            {
                // Arrange
                var financialInfo = FinancialInfoHelper.New(currency: curreny);

                // Act
                var dto = _mapper.Map<BillingDTO>(financialInfo);

                // Assert
                Assert.NotNull(dto);
                Assert.Equal(curreny.ToUpper(), dto.Currency);
            }
        }

        public class InvoiceDescriptionMapping
        {
            [Fact]
            public static void ToInvoiceDTO()
            {
                // Arrange
                var invoiceDescription = InvoiceDescriptionHelper.New();

                // Act
                var dto = _mapper.Map<InvoiceDTO>(invoiceDescription);

                // Assert
                Assert.NotNull(dto);
                Assert.Equal(invoiceDescription.Id, dto.Id);
                Assert.Equal(invoiceDescription.Cost, dto.Price);
                Assert.Equal(invoiceDescription.InvoiceDate, dto.Date);
            }
        }
    }
}
