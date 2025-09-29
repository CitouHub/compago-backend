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
        Task<List<InvoiceTagDTO>> GetInvoiceTagsAsync(int tagId);
        Task DeleteInvoiceTagAsync(InvoiceTagDTO invoiceTagDto);
    }

    public class InvoiceTagService(
        CompagoDbContext dbContext,
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
                    var dbInvoiceTag = mapper.Map<InvoiceTag>(invoiceTagDto);
                    await dbContext.InvoiceTags.AddAsync(dbInvoiceTag);
                    await dbContext.SaveChangesAsync();

                    return mapper.Map<InvoiceTagDTO>(dbInvoiceTag);
                }
                else
                {
                    throw new ServiceException(ExceptionType.ItemAlreadyExist, details: @$"{nameof(InvoiceTag)} with 
                    {nameof(InvoiceTag.TagId)} = {invoiceTagDto.TagId} and 
                    {nameof(InvoiceTag.InvoiceId)} = {invoiceTagDto.InvoiceId} already exists");
                }
            }
            else
            {
                throw new ServiceException(ExceptionType.ItemNotFound, details: @$"{nameof(Tag)} with 
                    {nameof(Tag.Id)} = {invoiceTagDto.TagId} not found");
            }
        }

        public async Task DeleteInvoiceTagAsync(InvoiceTagDTO invoiceTagDto)
        {
            var dbInvoiceTag = await dbContext.InvoiceTags.FirstOrDefaultAsync(_ => _.TagId == invoiceTagDto.TagId && _.InvoiceId == invoiceTagDto.InvoiceId);
            if (dbInvoiceTag != null)
            {
                dbContext.InvoiceTags.Remove(dbInvoiceTag);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                throw new ServiceException(ExceptionType.ItemNotFound, details: @$"{nameof(InvoiceTag)} with 
                    {nameof(InvoiceTag.TagId)} = {invoiceTagDto.TagId} and 
                    {nameof(InvoiceTag.InvoiceId)} = {invoiceTagDto.InvoiceId} not found");
            }
        }

        public async Task<List<InvoiceTagDTO>> GetInvoiceTagsAsync(int tagId)
        {
            var invoiceTags = await dbContext.InvoiceTags.Where(_ => _.TagId == tagId).ToListAsync();
            return mapper.Map<List<InvoiceTagDTO>>(invoiceTags);
        }
    }
}
