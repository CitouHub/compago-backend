using Compago.Common;
using Compago.Data;
using Compago.Service;
using Compago.Service.CustomeException;
using Compago.Test.Helper;
using Compago.Test.Helper.Domain;
using Microsoft.EntityFrameworkCore;

namespace Compago.Test.Service
{
    public class TagServiceTest : ServiceTest
    {
        public class AddTag
        {
            [Fact]
            public async Task InvalidColor()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var tagService = new TagService(dbContext, cacheService, _mapper);

                var color = "invalid";
                var tag = TagHelper.New(color: color);

                // Act
                var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                    tagService.AddTagAsync(tag));

                // Assert
                Assert.Equal(ExceptionType.InvalidRequest, exception.ExceptionType);
                Assert.Contains(nameof(Tag.Color), exception.Message);
                Assert.Contains(color, exception.Message);
            }

            [Fact]
            public async Task NameExists()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var tagService = new TagService(dbContext, cacheService, _mapper);

                var name = "TestTag";
                var tagDb = TagHelper.NewDb(name: name);
                await dbContext.Tags.AddAsync(tagDb);
                await dbContext.SaveChangesAsync();

                var tag = TagHelper.New(name: name);

                // Act
                var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                    tagService.AddTagAsync(tag));

                // Assert
                Assert.Equal(ExceptionType.ItemAlreadyExist, exception.ExceptionType);
                Assert.Contains(nameof(Tag), exception.Message);
                Assert.Contains(nameof(Tag.Name), exception.Message);
                Assert.Contains(name, exception.Message);
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var tagService = new TagService(dbContext, cacheService, _mapper);

                var tag = TagHelper.New(id: null);

                // Act
                var result = await tagService.AddTagAsync(tag);

                // Assert
                var dbTag = await dbContext.Tags.FirstOrDefaultAsync(_ => _.Id == 1);
                Assert.NotNull(dbTag);
                Assert.Equal(DefaultValues.TagColor, dbTag.Color);
                Assert.Equal(dbTag.CreatedBy, _cacheUserId);
                Assert.True(dbTag.CreatedAt > DateTime.UtcNow.AddMinutes(-1) && dbTag.CreatedAt < DateTime.UtcNow.AddMinutes(1));
            }
        }

        public class GetTags
        {
            [Fact]
            public async Task EmptyResult()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var tagService = new TagService(dbContext, cacheService, _mapper);

                // Act
                var result = await tagService.GetTagsAsync();

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public async Task Success_WithoutInvoiceTags()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var tagService = new TagService(dbContext, cacheService, _mapper);

                var tag1 = TagHelper.NewDb(id: 1, name: "Tag1");
                var tag2 = TagHelper.NewDb(id: 2, name: "Tag2");
                await dbContext.Tags.AddAsync(tag1);
                await dbContext.Tags.AddAsync(tag2);
                await dbContext.SaveChangesAsync();

                // Act
                var result = await tagService.GetTagsAsync();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                Assert.Contains(result, _ => _.Id == tag1.Id && _.Name == tag1.Name);
                Assert.Contains(result, _ => _.Id == tag2.Id && _.Name == tag2.Name);
            }

            [Fact]
            public async Task Success_WithInvoiceTags()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var tagService = new TagService(dbContext, cacheService, _mapper);

                var dbTag = TagHelper.NewDb();
                await dbContext.Tags.AddRangeAsync(dbTag);

                var dbInvoiceTag1 = InvoiceTagHelper.NewDb(invoiceId: "1", tagId: dbTag.Id);
                var dbInvoiceTag2 = InvoiceTagHelper.NewDb(invoiceId: "2", tagId: dbTag.Id);
                await dbContext.InvoiceTags.AddRangeAsync(dbInvoiceTag1, dbInvoiceTag2);
                await dbContext.SaveChangesAsync();

                // Act
                var result = await tagService.GetTagAsync(dbTag.Id);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.InvoiceTags.Count);
                Assert.Contains(result.InvoiceTags, _ => _.InvoiceId == dbInvoiceTag1.InvoiceId && _.TagId == dbInvoiceTag1.TagId);
                Assert.Contains(result.InvoiceTags, _ => _.InvoiceId == dbInvoiceTag2.InvoiceId && _.TagId == dbInvoiceTag2.TagId);
            }
        }

        public class GetTag
        {
            [Fact]
            public async Task TagNotFound()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var tagService = new TagService(dbContext, cacheService, _mapper);

                var tagDb = TagHelper.NewDb();
                await dbContext.Tags.AddAsync(tagDb);
                await dbContext.SaveChangesAsync();

                var requestId = tagDb.Id + 1;

                // Act
                var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                    tagService.GetTagAsync(requestId));

                // Assert
                Assert.Equal(ExceptionType.ItemNotFound, exception.ExceptionType);
                Assert.Contains(nameof(Tag), exception.Message);
                Assert.Contains(nameof(Tag.Id), exception.Message);
                Assert.Contains(requestId.ToString(), exception.Message);
            }

            [Fact]
            public async Task Success_WithoutInvoiceTags()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var tagService = new TagService(dbContext, cacheService, _mapper);

                var tagDb1 = TagHelper.NewDb(id: 1, name: "tag1");
                var tagDb2 = TagHelper.NewDb(id: 2, name: "tag2");
                await dbContext.Tags.AddRangeAsync(tagDb1, tagDb2);
                await dbContext.SaveChangesAsync();

                // Act
                var result = await tagService.GetTagAsync(tagDb2.Id);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(tagDb2.Id, result.Id);
                Assert.Equal(tagDb2.Name, result.Name);
                Assert.Equal(tagDb2.Color, result.Color);
            }

            [Fact]
            public async Task Success_WithInvoiceTags()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var tagService = new TagService(dbContext, cacheService, _mapper);

                var dbTag = TagHelper.NewDb();
                await dbContext.Tags.AddRangeAsync(dbTag);

                var dbInvoiceTag1 = InvoiceTagHelper.NewDb(invoiceId: "1", tagId: dbTag.Id);
                var dbInvoiceTag2 = InvoiceTagHelper.NewDb(invoiceId: "2", tagId: dbTag.Id);
                await dbContext.InvoiceTags.AddRangeAsync(dbInvoiceTag1, dbInvoiceTag2);
                await dbContext.SaveChangesAsync();

                // Act
                var result = await tagService.GetTagAsync(dbTag.Id);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.InvoiceTags.Count);
                Assert.Contains(result.InvoiceTags, _ => _.InvoiceId == dbInvoiceTag1.InvoiceId && _.TagId == dbInvoiceTag1.TagId);
                Assert.Contains(result.InvoiceTags, _ => _.InvoiceId == dbInvoiceTag2.InvoiceId && _.TagId == dbInvoiceTag2.TagId);
            }
        }

        public class UpdateTag
        {
            [Fact]
            public async Task InvalidColor()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var tagService = new TagService(dbContext, cacheService, _mapper);

                var color = "invalid";
                var tag = TagHelper.New(color: color);

                // Act
                var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                    tagService.UpdateTagAsync(tag));

                // Assert
                Assert.Equal(ExceptionType.InvalidRequest, exception.ExceptionType);
                Assert.Contains(nameof(Tag.Color), exception.Message);
                Assert.Contains(color, exception.Message);
            }

            [Fact]
            public async Task TagNotFound()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var tagService = new TagService(dbContext, cacheService, _mapper);

                var tagDb = TagHelper.NewDb();
                await dbContext.Tags.AddAsync(tagDb);
                await dbContext.SaveChangesAsync();

                var tag = TagHelper.New(id: (short)(tagDb.Id + 1));

                // Act
                var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                    tagService.UpdateTagAsync(tag));

                // Assert
                Assert.Equal(ExceptionType.ItemNotFound, exception.ExceptionType);
                Assert.Contains(nameof(Tag), exception.Message);
                Assert.Contains(nameof(Tag.Id), exception.Message);
                Assert.Contains(nameof(tag.Id), exception.Message);
            }

            [Fact]
            public async Task NameExists()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var tagService = new TagService(dbContext, cacheService, _mapper);

                var tagDb1 = TagHelper.NewDb(id: 1, name: "tag1");
                var tagDb2 = TagHelper.NewDb(id: 2, name: "tag2");
                await dbContext.Tags.AddRangeAsync(tagDb1, tagDb2);
                await dbContext.SaveChangesAsync();

                var tag = TagHelper.New(id: tagDb1.Id, name: tagDb2.Name);

                // Act
                var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                    tagService.UpdateTagAsync(tag));

                // Assert
                Assert.Equal(ExceptionType.ItemAlreadyExist, exception.ExceptionType);
                Assert.Contains(nameof(Tag), exception.Message);
                Assert.Contains(nameof(Tag.Name), exception.Message);
                Assert.Contains(nameof(tag.Name), exception.Message);
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var tagService = new TagService(dbContext, cacheService, _mapper);

                var tagDb1 = TagHelper.NewDb(id: 1, name: "tag1", color: "#001122");
                var tagDb2 = TagHelper.NewDb(id: 2, name: "tag2", color: "#334455");
                await dbContext.Tags.AddRangeAsync(tagDb1, tagDb2);
                await dbContext.SaveChangesAsync();

                var newName = "newName";
                var newColor = "#123456";
                var tag = TagHelper.New(id: tagDb2.Id, name: newName, color: newColor);

                // Act
                var result = await tagService.UpdateTagAsync(tag);

                // Assert
                var updateDbTag = await dbContext.Tags.FirstOrDefaultAsync(_ => _.Id == tagDb2.Id);
                Assert.NotNull(result);
                Assert.Equal(newName, result.Name);
                Assert.Equal(newColor, result.Color);

                Assert.NotNull(updateDbTag);
                Assert.Equal(newName, updateDbTag.Name);
                Assert.Equal(newColor, updateDbTag.Color);

                Assert.Equal(updateDbTag.UpdatedBy, _cacheUserId);
                Assert.True(updateDbTag.UpdatedAt > DateTime.UtcNow.AddMinutes(-1) && updateDbTag.UpdatedAt < DateTime.UtcNow.AddMinutes(1));
            }
        }

        public class DeleteTag
        {
            [Fact]
            public async Task TagNotFound()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var tagService = new TagService(dbContext, cacheService, _mapper);

                var tagDb = TagHelper.NewDb();
                await dbContext.Tags.AddAsync(tagDb);
                await dbContext.SaveChangesAsync();

                var requestId = tagDb.Id + 1;

                // Act
                var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                    tagService.DeleteTagAsync(requestId));

                // Assert
                Assert.Equal(ExceptionType.ItemNotFound, exception.ExceptionType);
                Assert.Contains(nameof(Tag), exception.Message);
                Assert.Contains(nameof(Tag.Id), exception.Message);
                Assert.Contains(requestId.ToString(), exception.Message);
            }

            [Fact]
            public async Task Success_WithoutInvoiceTags()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var tagService = new TagService(dbContext, cacheService, _mapper);

                var tagDb1 = TagHelper.NewDb(id: 1, name: "tag1");
                var tagDb2 = TagHelper.NewDb(id: 2, name: "tag2");
                await dbContext.Tags.AddRangeAsync(tagDb1, tagDb2);
                await dbContext.SaveChangesAsync();

                // Act
                await tagService.DeleteTagAsync(tagDb2.Id);

                // Assert
                var dbTag1 = await dbContext.Tags.FirstOrDefaultAsync(_ => _.Id == tagDb1.Id);
                var dbTag2 = await dbContext.Tags.FirstOrDefaultAsync(_ => _.Id == tagDb2.Id);
                Assert.NotNull(dbTag1);
                Assert.Null(dbTag2);
            }

            [Fact]
            public async Task Success_WithInvoiceTags()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var tagService = new TagService(dbContext, cacheService, _mapper);

                var dbTag = TagHelper.NewDb();
                await dbContext.Tags.AddRangeAsync(dbTag);

                var dbInvoiceTag1 = InvoiceTagHelper.NewDb(invoiceId: "1", tagId: dbTag.Id);
                var dbInvoiceTag2 = InvoiceTagHelper.NewDb(invoiceId: "2", tagId: dbTag.Id);
                await dbContext.InvoiceTags.AddRangeAsync(dbInvoiceTag1, dbInvoiceTag2);
                await dbContext.SaveChangesAsync();

                // Act
                await tagService.DeleteTagAsync(dbTag.Id);

                // Assert
                var deletedDbTag = await dbContext.Tags.FirstOrDefaultAsync(_ => _.Id == dbTag.Id);
                var deletedDbInvoiceTag1 = await dbContext.InvoiceTags.FirstOrDefaultAsync(_ => _.InvoiceId == dbInvoiceTag1.InvoiceId);
                var deletedDbInvoiceTag2 = await dbContext.InvoiceTags.FirstOrDefaultAsync(_ => _.InvoiceId == dbInvoiceTag2.InvoiceId);
                Assert.NotNull(dbTag);
                Assert.Null(deletedDbInvoiceTag1);
                Assert.Null(deletedDbInvoiceTag2);
            }
        }
    }
}
