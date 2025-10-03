using AutoMapper;
using Compago.Data;
using Compago.Domain;
using Compago.Service.CustomeException;
using Microsoft.EntityFrameworkCore;

namespace Compago.Service
{
    public interface IInvoiceTagService
    {
        Task<InvoiceTagDTO> AddInvoiceTagAsync(InvoiceTagDTO invoiceTagDto);
        Task<List<InvoiceTagDTO>?> GetInvoiceTagsAsync(int tagId);
        Task DeleteInvoiceTagAsync(string invoiceId, short tagId);
    }

    public class InvoiceTagService(
        CompagoDbContext dbContext,
        ICacheService cacheService,
        IMapper mapper) : IInvoiceTagService
    {
        public async Task<InvoiceTagDTO> AddInvoiceTagAsync(InvoiceTagDTO invoiceTagDto)
        {
            var tagExists = await dbContext.Tags.AnyAsync(_ => _.Id == invoiceTagDto.TagId);
            if (tagExists == true)
            {
                var invoiceTagExists = await dbContext.InvoiceTags.AnyAsync(_ => _.TagId == invoiceTagDto.TagId && _.InvoiceId == invoiceTagDto.InvoiceId);
                if (invoiceTagExists == false)
                {
                    var userSecurityCredentials = cacheService.Get<UserSecurityCredentialsDTO>();

                    var dbInvoiceTag = mapper.Map<InvoiceTag>(invoiceTagDto);
                    dbInvoiceTag.CreatedAt = DateTime.UtcNow;
                    dbInvoiceTag.CreatedBy = userSecurityCredentials!.Id;
                    await dbContext.InvoiceTags.AddAsync(dbInvoiceTag);
                    await dbContext.SaveChangesAsync();

                    return mapper.Map<InvoiceTagDTO>(dbInvoiceTag);
                }
                else
                {
                    throw new ServiceException(ExceptionType.ItemAlreadyExist, details: @$"{nameof(InvoiceTag)} with 
                        {nameof(InvoiceTag.InvoiceId)} = {invoiceTagDto.InvoiceId} and
                        {nameof(InvoiceTag.TagId)} = {invoiceTagDto.TagId} already exists");
                }
            }
            else
            {
                throw new ServiceException(ExceptionType.ItemNotFound, details: @$"{nameof(Tag)} with 
                    {nameof(Tag.Id)} = {invoiceTagDto.TagId} not found");
            }
        }

        public async Task DeleteInvoiceTagAsync(string invoiceId, short tagId)
        {
            var dbInvoiceTag = await dbContext.InvoiceTags.FirstOrDefaultAsync(_ => _.InvoiceId == invoiceId && _.TagId == tagId);
            if (dbInvoiceTag != null)
            {
                dbContext.InvoiceTags.Remove(dbInvoiceTag);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                throw new ServiceException(ExceptionType.ItemNotFound, details: @$"{nameof(InvoiceTag)} with 
                    {nameof(InvoiceTag.InvoiceId)} = {invoiceId} and
                    {nameof(InvoiceTag.TagId)} = {tagId} not found");
            }
        }

        public async Task<List<InvoiceTagDTO>?> GetInvoiceTagsAsync(int tagId)
        {
            var invoiceTags = await dbContext.InvoiceTags.Where(_ => _.TagId == tagId).ToListAsync();
            return invoiceTags.Count != 0 ? mapper.Map<List<InvoiceTagDTO>>(invoiceTags) : null;
        }
    }
}
