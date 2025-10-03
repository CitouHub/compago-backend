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
        public class UpdateInvoiceTag
        {
            [Fact]
            public async Task TagNotFound()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var invoiceTagService = new InvoiceTagService(dbContext, cacheService, _mapper);

                var dbTag = TagHelper.NewDb();
                await dbContext.Tags.AddAsync(dbTag);
                await dbContext.SaveChangesAsync();
                var invalidTagId = (short)(dbTag.Id + 1);

                // Act
                var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                    invoiceTagService.UpdateInvoiceTagAsync("1", [invalidTagId]));

                // Assert
                Assert.Equal(ExceptionType.ItemNotFound, exception.ExceptionType);
                Assert.Contains(nameof(Tag), exception.Message);
                Assert.Contains(nameof(Tag.Id), exception.Message);
                Assert.Contains(invalidTagId.ToString(), exception.Message);
            }

            [Fact]
            public async Task OneTagNotFound()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var invoiceTagService = new InvoiceTagService(dbContext, cacheService, _mapper);

                var dbTag1 = TagHelper.NewDb(id: 1);
                var dbTag2 = TagHelper.NewDb(id: 2);
                await dbContext.Tags.AddRangeAsync(dbTag1, dbTag2);
                await dbContext.SaveChangesAsync();
                var invalidTagId = (short)(dbTag2.Id + 1);

                // Act
                var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                    invoiceTagService.UpdateInvoiceTagAsync("1", [dbTag1.Id, invalidTagId]));

                // Assert
                Assert.Equal(ExceptionType.ItemNotFound, exception.ExceptionType);
                Assert.Contains(nameof(Tag), exception.Message);
                Assert.Contains(nameof(Tag.Id), exception.Message);
                Assert.Contains(invalidTagId.ToString(), exception.Message);
                Assert.DoesNotContain(dbTag1.Id.ToString(), exception.Message);
            }

            [Fact]
            public async Task Success_WithoutInvoiceTags()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var invoiceTagService = new InvoiceTagService(dbContext, cacheService, _mapper);

                var dbTag1 = TagHelper.NewDb(id: 1);
                var dbTag2 = TagHelper.NewDb(id: 2);
                var dbTag3 = TagHelper.NewDb(id: 3);
                await dbContext.Tags.AddRangeAsync(dbTag1, dbTag2, dbTag3);
                await dbContext.SaveChangesAsync();

                var invoiceId = "1";
                var tagIds = new List<short>() { dbTag1.Id, dbTag3.Id };

                // Act
                var result = await invoiceTagService.UpdateInvoiceTagAsync(invoiceId, tagIds);

                // Assert
                var addedDbInvoiceTags = await dbContext.InvoiceTags.Where(_ => _.InvoiceId == invoiceId).ToListAsync();
                Assert.NotNull(addedDbInvoiceTags);
                Assert.Equal(2, addedDbInvoiceTags.Count);
                Assert.NotNull(addedDbInvoiceTags.FirstOrDefault(_ => _.TagId == dbTag1.Id));
                Assert.NotNull(addedDbInvoiceTags.FirstOrDefault(_ => _.TagId == dbTag3.Id));
                Assert.True(addedDbInvoiceTags.All(_ => _.CreatedBy == _cacheUserId));
                Assert.True(addedDbInvoiceTags.All(_ => _.CreatedAt > DateTime.UtcNow.AddMinutes(-1) && _.CreatedAt < DateTime.UtcNow.AddMinutes(1)));
            }

            [Fact]
            public async Task Success_WithInvoiceTags()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var invoiceTagService = new InvoiceTagService(dbContext, cacheService, _mapper);

                var dbTag1 = TagHelper.NewDb(id: 1);
                var dbTag2 = TagHelper.NewDb(id: 2);
                var dbTag3 = TagHelper.NewDb(id: 3);
                await dbContext.Tags.AddRangeAsync(dbTag1, dbTag2, dbTag3);

                var invoiceId = "1";
                var dbInvoiceTag1 = InvoiceTagHelper.NewDb(invoiceId, dbTag2.Id);
                var dbInvoiceTag2 = InvoiceTagHelper.NewDb(invoiceId, dbTag3.Id);
                await dbContext.InvoiceTags.AddRangeAsync(dbInvoiceTag1, dbInvoiceTag1);
                await dbContext.SaveChangesAsync();

                var tagIds = new List<short>() { dbTag1.Id, dbTag3.Id };

                // Act
                var result = await invoiceTagService.UpdateInvoiceTagAsync(invoiceId, tagIds);

                // Assert
                var addedDbInvoiceTags = await dbContext.InvoiceTags.Where(_ => _.InvoiceId == invoiceId).ToListAsync();
                Assert.NotNull(addedDbInvoiceTags);
                Assert.Equal(2, addedDbInvoiceTags.Count);
                Assert.NotNull(addedDbInvoiceTags.FirstOrDefault(_ => _.TagId == dbTag1.Id));
                Assert.NotNull(addedDbInvoiceTags.FirstOrDefault(_ => _.TagId == dbTag3.Id));
                Assert.True(addedDbInvoiceTags.All(_ => _.CreatedBy == _cacheUserId));
                Assert.True(addedDbInvoiceTags.All(_ => _.CreatedAt > DateTime.UtcNow.AddMinutes(-1) && _.CreatedAt < DateTime.UtcNow.AddMinutes(1)));
            }
        }

        public class GetInvoiceTags
        {
            [Fact]
            public async Task EmptyResult()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var invoiceTagService = new InvoiceTagService(dbContext, cacheService, _mapper);

                // Act
                var result = await invoiceTagService.GetInvoiceTagsAsync("1");

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var invoiceTagService = new InvoiceTagService(dbContext, cacheService, _mapper);

                var dbTag1 = TagHelper.NewDb(id: 1, name: "tag1", color: "#001122");
                var dbTag2 = TagHelper.NewDb(id: 2, name: "tag2", color: "#334455");
                await dbContext.Tags.AddRangeAsync(dbTag1, dbTag2);

                var invoiceId1 = "1";
                var invoiceId2 = "2";
                var dbInvoiceTag1 = InvoiceTagHelper.NewDb(invoiceId: invoiceId1, tagId: dbTag1.Id);
                var dbInvoiceTag2 = InvoiceTagHelper.NewDb(invoiceId: invoiceId1, tagId: dbTag2.Id);
                var dbInvoiceTag3 = InvoiceTagHelper.NewDb(invoiceId: invoiceId2, tagId: dbTag1.Id);
                await dbContext.InvoiceTags.AddRangeAsync(dbInvoiceTag1, dbInvoiceTag2, dbInvoiceTag3);
                await dbContext.SaveChangesAsync();

                // Act
                var result = await invoiceTagService.GetInvoiceTagsAsync(dbInvoiceTag1.InvoiceId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);

                var invoiceTag1 = result.FirstOrDefault(_ => _.InvoiceId == invoiceId1 && _.TagId == dbTag1.Id);
                Assert.NotNull(invoiceTag1);
                Assert.Equal(dbTag1.Name, invoiceTag1.TagName);
                Assert.Equal(dbTag1.Color, invoiceTag1.TagColor);

                var invoiceTag2 = result.FirstOrDefault(_ => _.InvoiceId == invoiceId1 && _.TagId == dbTag2.Id);
                Assert.NotNull(invoiceTag2);
                Assert.Equal(dbTag2.Name, invoiceTag2.TagName);
                Assert.Equal(dbTag2.Color, invoiceTag2.TagColor);
            }
        }
    }
}
