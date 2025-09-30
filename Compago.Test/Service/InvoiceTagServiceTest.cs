using Compago.Data;
using Compago.Service;
using Compago.Service.CustomeException;
using Compago.Test.Helper;
using Compago.Test.Helper.Domain;
using Microsoft.EntityFrameworkCore;

namespace Compago.Test.Service
{
    public class InvoiceTagServiceTest : ServiceTest
    {
        public class AddInvoiceTag
        {
            [Fact]
            public async Task TagNotFound()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var invoiceTagService = new InvoiceTagService(dbContext, _mapper);

                var dbTag = TagHelper.NewDb();
                await dbContext.Tags.AddAsync(dbTag);
                await dbContext.SaveChangesAsync();

                var invoiceTag = InvoiceTagHelper.New(tagId: (short)(dbTag.Id + 1));

                // Act
                var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                    invoiceTagService.AddInvoiceTagAsync(invoiceTag));

                // Assert
                Assert.Equal(ExceptionType.ItemNotFound, exception.ExceptionType);
                Assert.Contains(nameof(Tag), exception.Message);
                Assert.Contains(nameof(Tag.Id), exception.Message);
                Assert.Contains(invoiceTag.TagId.ToString(), exception.Message);
            }

            [Fact]
            public async Task InvoiceTagExists()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var invoiceTagService = new InvoiceTagService(dbContext, _mapper);

                var dbTag = TagHelper.NewDb();
                await dbContext.Tags.AddAsync(dbTag);

                var dbInvoiceTag = InvoiceTagHelper.NewDb(tagId: dbTag.Id);
                await dbContext.InvoiceTags.AddAsync(dbInvoiceTag);
                await dbContext.SaveChangesAsync();

                var invoiceTag = InvoiceTagHelper.New(tagId: dbInvoiceTag.TagId);

                // Act
                var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                    invoiceTagService.AddInvoiceTagAsync(invoiceTag));

                // Assert
                Assert.Equal(ExceptionType.ItemAlreadyExist, exception.ExceptionType);
                Assert.Contains(nameof(InvoiceTag), exception.Message);
                Assert.Contains(nameof(InvoiceTag.InvoiceId), exception.Message);
                Assert.Contains(nameof(InvoiceTag.TagId), exception.Message);
                Assert.Contains(invoiceTag.InvoiceId, exception.Message);
                Assert.Contains(invoiceTag.TagId.ToString(), exception.Message);
            }

            [Theory]
            [InlineData("i1", 1, "i2", 1)]
            [InlineData("i1", 1, "i1", 2)]
            [InlineData("i1", 2, "i2", 2)]
            [InlineData("i1", 2, "i1", 1)]
            [InlineData("i2", 1, "i1", 1)]
            [InlineData("i2", 1, "i2", 2)]
            [InlineData("i2", 2, "i1", 2)]
            [InlineData("i2", 2, "i2", 1)]
            public async Task Success(string givenInvoiceId, short givenTagId, string existingInvoiceId, short existingTagId)
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var invoiceTagService = new InvoiceTagService(dbContext, _mapper);

                var dbTag1 = TagHelper.NewDb(id: 1);
                var dbTag2 = TagHelper.NewDb(id: 2);
                await dbContext.Tags.AddRangeAsync(dbTag1, dbTag2);

                var dbInvoiceTag = InvoiceTagHelper.NewDb(invoiceId: existingInvoiceId, tagId: existingTagId);
                await dbContext.InvoiceTags.AddAsync(dbInvoiceTag);
                await dbContext.SaveChangesAsync();

                var invoiceTag = InvoiceTagHelper.New(invoiceId: givenInvoiceId, tagId: givenTagId);

                // Act
                var result = await invoiceTagService.AddInvoiceTagAsync(invoiceTag);

                // Assert
                var addedDbInvoiceTag = await dbContext.InvoiceTags.FirstOrDefaultAsync(_ => _.InvoiceId == givenInvoiceId && _.TagId == givenTagId);
                Assert.NotNull(addedDbInvoiceTag);
            }
        }

        public class GetInvoiceTags
        {
            [Fact]
            public async Task EmptyResult()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var invoiceTagService = new InvoiceTagService(dbContext, _mapper);

                // Act
                var result = await invoiceTagService.GetInvoiceTagsAsync(1);

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var invoiceTagService = new InvoiceTagService(dbContext, _mapper);

                var dbTag = TagHelper.NewDb();
                await dbContext.Tags.AddAsync(dbTag);

                var dbInvoiceTag1 = InvoiceTagHelper.NewDb(invoiceId: "1", tagId: dbTag.Id);
                var dbInvoiceTag2 = InvoiceTagHelper.NewDb(invoiceId: "2", tagId: dbTag.Id);
                await dbContext.InvoiceTags.AddRangeAsync(dbInvoiceTag1, dbInvoiceTag2);
                await dbContext.SaveChangesAsync();

                // Act
                var result = await invoiceTagService.GetInvoiceTagsAsync(dbTag.Id);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                Assert.Contains(result, _ => _.InvoiceId == dbInvoiceTag1.InvoiceId && _.TagId == dbInvoiceTag1.TagId);
                Assert.Contains(result, _ => _.InvoiceId == dbInvoiceTag2.InvoiceId && _.TagId == dbInvoiceTag2.TagId);
            }
        }

        public class DeleteInvoiceTag
        {
            [Theory]
            [InlineData("i1", 1, "i2", 1)]
            [InlineData("i1", 1, "i1", 2)]
            public async Task InvoiceTagNotFound(string givenInvoiceId, short givenTagId, string existingInvoiceId, short existingTagId)
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var invoiceTagService = new InvoiceTagService(dbContext, _mapper);

                var dbTag = TagHelper.NewDb(id: existingTagId);
                await dbContext.Tags.AddAsync(dbTag);

                var invoiceTagDb = InvoiceTagHelper.NewDb(invoiceId: existingInvoiceId, tagId: dbTag.Id);
                await dbContext.InvoiceTags.AddAsync(invoiceTagDb);
                await dbContext.SaveChangesAsync();

                // Act
                var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                    invoiceTagService.DeleteInvoiceTagAsync(givenInvoiceId, givenTagId));

                // Assert
                Assert.Equal(ExceptionType.ItemNotFound, exception.ExceptionType);
                Assert.Contains(nameof(InvoiceTag), exception.Message);
                Assert.Contains(nameof(InvoiceTag.InvoiceId), exception.Message);
                Assert.Contains(nameof(InvoiceTag.TagId), exception.Message);
                Assert.Contains(givenInvoiceId, exception.Message);
                Assert.Contains(givenTagId.ToString(), exception.Message);
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var invoiceTagService = new InvoiceTagService(dbContext, _mapper);

                var dbTag = TagHelper.NewDb();
                await dbContext.Tags.AddAsync(dbTag);

                var dbInvoiceTag1 = InvoiceTagHelper.NewDb(invoiceId: "1", tagId: dbTag.Id);
                var dbInvoiceTag2 = InvoiceTagHelper.NewDb(invoiceId: "2", tagId: dbTag.Id);
                await dbContext.InvoiceTags.AddRangeAsync(dbInvoiceTag1, dbInvoiceTag2);
                await dbContext.SaveChangesAsync();

                // Act
                await invoiceTagService.DeleteInvoiceTagAsync(dbInvoiceTag2.InvoiceId, dbInvoiceTag2.TagId);

                // Assert
                var deletedDbInvoiceTag1 = await dbContext.InvoiceTags.FirstOrDefaultAsync(_ => _.InvoiceId == dbInvoiceTag2.InvoiceId && _.TagId == dbTag.Id);
                var noneDeletedDbInvoiceTag1 = await dbContext.InvoiceTags.FirstOrDefaultAsync(_ => _.InvoiceId == dbInvoiceTag1.InvoiceId && _.TagId == dbTag.Id);
                Assert.Null(deletedDbInvoiceTag1);
                Assert.NotNull(noneDeletedDbInvoiceTag1);
            }
        }
    }
}
