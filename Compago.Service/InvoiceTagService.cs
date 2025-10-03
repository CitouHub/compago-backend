using AutoMapper;
using Compago.Data;
using Compago.Domain;
using Compago.Service.CustomeException;
using Microsoft.EntityFrameworkCore;

namespace Compago.Service
{
    public interface IInvoiceTagService
    {
        Task<List<InvoiceTagDTO>?> UpdateInvoiceTagAsync(string invoiceId, List<short> tagIds);
        Task<List<InvoiceTagDTO>?> GetInvoiceTagsAsync(string invoiceId);
    }

    public class InvoiceTagService(
        CompagoDbContext dbContext,
        ICacheService cacheService,
        IMapper mapper) : IInvoiceTagService
    {
        public async Task<List<InvoiceTagDTO>?> UpdateInvoiceTagAsync(string invoiceId, List<short> tagIds)
        {
            var requestTags = await dbContext.Tags
                .Where(_ => tagIds.Contains(_.Id) == true)
                .Select(_ => _.Id)
                .ToListAsync();
            if (requestTags.Count == tagIds.Count)
            {
                var currentTags = await dbContext.InvoiceTags.Where(_ => _.InvoiceId == invoiceId).ToListAsync();
                dbContext.InvoiceTags.RemoveRange(currentTags);
                await dbContext.SaveChangesAsync();

                var userSecurityCredentials = cacheService.Get<UserSecurityCredentialsDTO>();

                var dbInvoiceTags = new List<InvoiceTag>();
                foreach (var tagId in tagIds)
                {
                    dbInvoiceTags.Add(new InvoiceTag()
                    {
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = userSecurityCredentials!.Id,
                        InvoiceId = invoiceId,
                        TagId = tagId,
                    });
                }
                await dbContext.InvoiceTags.AddRangeAsync(dbInvoiceTags);
                await dbContext.SaveChangesAsync();

                var addedDbInvoiceTags = await dbContext.InvoiceTags
                    .Include(_ => _.Tag)
                    .Where(_ => _.InvoiceId == invoiceId && tagIds.Contains(_.TagId))
                    .ToListAsync();

                return addedDbInvoiceTags.Count > 0 ? mapper.Map<List<InvoiceTagDTO>>(addedDbInvoiceTags) : null;
            }
            else
            {
                var noneExistingIds = tagIds.Where(_ => requestTags.Contains(_) == false).ToList();
                throw new ServiceException(ExceptionType.ItemNotFound, details: @$"{nameof(Tag)} with 
                    {nameof(Tag.Id)}(s) = {string.Join(", ", noneExistingIds.Select(_ => _.ToString()))} not found");
            }
        }

        public async Task<List<InvoiceTagDTO>?> GetInvoiceTagsAsync(string invoiceId)
        {
            var invoiceTags = await dbContext.InvoiceTags.Include(_ => _.Tag).Where(_ => _.InvoiceId == invoiceId).ToListAsync();
            return invoiceTags.Count != 0 ? mapper.Map<List<InvoiceTagDTO>>(invoiceTags) : null;
        }
    }
}
