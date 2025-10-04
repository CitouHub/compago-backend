using AutoMapper;
using Compago.Domain;
using Compago.Test.Helper;
using Compago.Test.Helper.Domain.ExternalSource.MicrosoftAzure;
using System.Globalization;

namespace Compago.Test.Mapping
{
    public class MicrosoftAzureMappingTest
    {
        private static readonly IMapper _mapper = MapperHelper.DefineMapper();

        public class InvoiceMapping
        {
            [Fact]
            public static void ToDTO_WithoutBill()
            {
                // Arrange
                var monthly = MonthlyHelper.New(issueDate: new DateTime(2025, 01, 01));

                // Act
                var dto = _mapper.Map<InvoiceDTO>(monthly);

                // Assert
                Assert.NotNull(dto);
                Assert.Equal(monthly.IssueDate, dto.Date);
                Assert.Equal(0, dto.Price);
                Assert.Null(dto.Id);
            }

            [Fact]
            public static void ToDTO_WithBill()
            {
                // Arrange
                var monthly = MonthlyHelper.New(issueDate: new DateTime(2025, 01, 01));
                monthly.Bill = BillHelper.New();

                // Act
                var dto = _mapper.Map<InvoiceDTO>(monthly);

                // Assert
                Assert.NotNull(dto);
                Assert.Equal(monthly.IssueDate, dto.Date);
                Assert.Equal(double.Parse(monthly.Bill.MoneyToPay.Replace(",", "."), CultureInfo.InvariantCulture), dto.Price);
                Assert.Equal(monthly.Bill.Reference.ToString(), dto.Id);
            }
        }
    }
}
