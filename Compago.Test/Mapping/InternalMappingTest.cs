using AutoMapper;
using Compago.Data;
using Compago.Domain;
using Compago.Test.Helper;
using Compago.Test.Helper.Domain;

namespace Compago.Test.Mapping
{
    public class InternalMappingTest
    {
        private static readonly IMapper _mapper = MapperHelper.DefineMapper();

        public class UserMapping
        {
            [Fact]
            public void ToDTO()
            {
                // Arrange
                var dbRole = RoleHelper.NewDb();
                var dbUser = UserHelper.NewDb(roleId: (Compago.Common.Role)dbRole.Id);
                dbUser.Role = dbRole;

                // Act
                var dto = _mapper.Map<UserDTO>(dbUser);

                // Assert
                Assert.NotNull(dto);
                Assert.Equal(dbRole.Id, (short)dto.RoleId);
                Assert.Equal(dbRole.Name, dto.RoleName);
            }

            [Fact]
            public void ToDb()
            {
                // Arrange
                var user = UserHelper.New(roleId: Compago.Common.Role.Admin);

                // Act
                var db = _mapper.Map<User>(user);

                // Assert
                Assert.NotNull(db);
                Assert.Equal(user.RoleId, (Compago.Common.Role)db.RoleId);
            }
        }

        public class TagMapping
        {
            [Fact]
            public void ToDTO()
            {
                // Arrange
                var dbTag = TagHelper.NewDb();

                // Act
                var dto = _mapper.Map<TagDTO>(dbTag);

                // Assert
                Assert.NotNull(dto);
            }

            [Fact]
            public void ToDb()
            {
                // Arrange
                var tag = TagHelper.New();

                // Act
                var db = _mapper.Map<Tag>(tag);

                // Assert
                Assert.NotNull(db);
            }
        }

        public class InvoiceTagMapping
        {
            [Fact]
            public void ToDTO()
            {
                // Arrange
                var dbInvoiceTag = InvoiceTagHelper.NewDb();

                // Act
                var dto = _mapper.Map<InvoiceTagDTO>(dbInvoiceTag);

                // Assert
                Assert.NotNull(dto);
            }

            [Fact]
            public void ToDb()
            {
                // Arrange
                var invoiceTag = InvoiceTagHelper.New();

                // Act
                var db = _mapper.Map<InvoiceTag>(invoiceTag);

                // Assert
                Assert.NotNull(db);
            }
        }
    }
}
