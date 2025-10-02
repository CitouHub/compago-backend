using AutoMapper;
using Compago.Common.Extension;
using Compago.Data;
using Compago.Domain;
using Compago.Service.CustomeException;
using Microsoft.EntityFrameworkCore;

namespace Compago.Service
{
    public interface ITagService
    {
        Task<TagDTO> AddTagAsync(TagDTO tagDto);
        Task<List<TagDTO>?> GetTagsAsync();
        Task<TagDTO> GetTagAsync(int tagId);
        Task<TagDTO> UpdateTagAsync(TagDTO tagDto);
        Task DeleteTagAsync(int tagId);
    }

    public class TagService(
        CompagoDbContext dbContext,
        IMapper mapper) : ITagService
    {
        public async Task<TagDTO> AddTagAsync(TagDTO tagDto)
        {
            var isValidColorCode = tagDto.Color?.IsColorCode() ?? true;
            if (isValidColorCode == true)
            {
                var tagExists = await dbContext.Tags.AnyAsync(_ => _.Name == tagDto.Name);
                if (tagExists == false)
                {
                    var dbTag = mapper.Map<Tag>(tagDto);
                    await dbContext.Tags.AddAsync(dbTag);
                    await dbContext.SaveChangesAsync();

                    return mapper.Map<TagDTO>(dbTag);
                }
                else
                {
                    throw new ServiceException(ExceptionType.ItemAlreadyExist, details: @$"{nameof(Tag)} with 
                        {nameof(Tag.Name)} = {tagDto.Name} already exists");
                }
            } 
            else
            {
                throw new ServiceException(ExceptionType.InvalidRequest, details: $"Given {nameof(Tag.Color)} = {tagDto.Color} is invalid");
            }
            
        }

        public async Task DeleteTagAsync(int tagId)
        {
            var dbTag = await dbContext.Tags.Include(_ => _.InvoiceTags).FirstOrDefaultAsync(_ => _.Id == tagId);
            if (dbTag != null)
            {
                dbContext.InvoiceTags.RemoveRange(dbTag.InvoiceTags);
                await dbContext.SaveChangesAsync();

                dbContext.Tags.Remove(dbTag);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                throw new ServiceException(ExceptionType.ItemNotFound, details: @$"{nameof(Tag)} with 
                    {nameof(Tag.Id)} = {tagId} not found");
            }
        }

        public async Task<TagDTO> GetTagAsync(int tagId)
        {
            var dbTag = await dbContext.Tags.Include(_ => _.InvoiceTags).FirstOrDefaultAsync(_ => _.Id == tagId);
            if (dbTag != null)
            {
                return mapper.Map<TagDTO>(dbTag);
            }
            else
            {
                throw new ServiceException(ExceptionType.ItemNotFound, details: @$"{nameof(Tag)} with 
                    {nameof(Tag.Id)} = {tagId} not found");
            }
        }

        public async Task<List<TagDTO>?> GetTagsAsync()
        {
            var tags = await dbContext.Tags.Include(_ => _.InvoiceTags).ToListAsync();
            return tags.Count != 0 ? mapper.Map<List<TagDTO>>(tags) : null;
        }

        public async Task<TagDTO> UpdateTagAsync(TagDTO tagDto)
        {
            var dbTag = await dbContext.Tags.FirstOrDefaultAsync(_ => _.Id == tagDto.Id);
            if (dbTag != null)
            {
                var otherTagExists = await dbContext.Tags.AnyAsync(_ => _.Name == tagDto.Name && _.Id != tagDto.Id);
                if (otherTagExists == false)
                {
                        mapper.Map(tagDto, dbTag);
                        await dbContext.SaveChangesAsync();

                        return mapper.Map<TagDTO>(dbTag);
                }
                else
                {
                    throw new ServiceException(ExceptionType.ItemAlreadyExist, details: @$"{nameof(Tag)} with 
                        {nameof(Tag.Name)} = {tagDto.Name} already exists");
                }
            }
            else
            {
                throw new ServiceException(ExceptionType.ItemNotFound, details: @$"{nameof(Tag)} with 
                    {nameof(Tag.Id)} = {tagDto.Id} not found");
            }
        }
    }
}
