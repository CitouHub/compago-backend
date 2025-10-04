using AutoMapper;
using Compago.Domain;
using Compago.Test.Helper;
using Compago.Test.Helper.Domain.ExternalSource.GSuite;

namespace Compago.Test.Mapping
{
    public class GSuiteMappingTest
    {
        private static readonly IMapper _mapper = MapperHelper.DefineMapper();

        public class InvoiceMapping
        {
            [Fact]
            public static void ToDTO()
            {
                // Arrange
                var invoiceDescription = InvoiceDescriptionHelper.New(invoiceDate: new DateTime(2025, 01, 01));

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
