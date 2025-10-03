using AutoMapper;
using Azure;
using Compago.Common;
using Compago.Data;
using Compago.Domain;
using Compago.Service.CustomeException;
using Microsoft.EntityFrameworkCore;

namespace Compago.Service
{
    public interface IUserService
    {
        Task<UserDTO> AddUserAsync(UserDTO userDto);
        Task<List<UserDTO>?> GetUsersAsync();
        Task<UserDTO> GetUserAsync(int userId);
        Task<UserDTO> UpdateUserAsync(UserDTO userDto);
        Task DeleteUserAsync(int userId);
        Task<UserSecurityCredentialsDTO> GetUserSecurityCredentialsAsync(string username);
    }

    public class UserService(
        CompagoDbContext dbContext,
        ICacheService cacheService,
        IMapper mapper) : IUserService
    {
        public async Task<UserDTO> AddUserAsync(UserDTO userDto)
        {
            if (userDto.Password != null)
            {
                var userExists = await dbContext.Users.AnyAsync(_ => _.Username == userDto.Username);
                if (userExists == false)
                {
                    var userSecurityCredentials = cacheService.Get<UserSecurityCredentialsDTO>();

                    var dbUser = mapper.Map<User>(userDto);
                    var (hash, salt) = PasswordHandler.HashPassword(userDto.Password);
                    dbUser.PasswordHash = hash;
                    dbUser.PasswordHashSalt = salt;
                    dbUser.CreatedAt = DateTime.UtcNow;
                    dbUser.CreatedBy = userSecurityCredentials!.Id;
                    await dbContext.Users.AddAsync(dbUser);
                    await dbContext.SaveChangesAsync();

                    dbUser = await dbContext.Users.Include(_ => _.Role).FirstOrDefaultAsync(_ => _.Id == dbUser.Id);
                    return mapper.Map<UserDTO>(dbUser);
                }
                else
                {
                    throw new ServiceException(ExceptionType.ItemAlreadyExist, details: @$"{nameof(User)} with 
                    {nameof(User.Username)} = {userDto.Username} already exists");
                }
            }
            else
            {
                throw new ServiceException(ExceptionType.InvalidRequest, details: @$"{nameof(User)} is missing {nameof(UserDTO.Password)}");
            }
        }

        public async Task DeleteUserAsync(int userId)
        {
            var dbUser = await dbContext.Users.FirstOrDefaultAsync(_ => _.Id == userId);
            if (dbUser != null)
            {
                dbContext.Users.Remove(dbUser);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                throw new ServiceException(ExceptionType.ItemNotFound, details: @$"{nameof(User)} with 
                    {nameof(User.Id)} = {userId} not found");
            }
        }

        public async Task<UserDTO> GetUserAsync(int userId)
        {
            var dbUser = await dbContext.Users.Include(_ => _.Role).FirstOrDefaultAsync(_ => _.Id == userId);
            if (dbUser != null)
            {
                return mapper.Map<UserDTO>(dbUser);
            }
            else
            {
                throw new ServiceException(ExceptionType.ItemNotFound, details: @$"{nameof(User)} with 
                    {nameof(User.Id)} = {userId} not found");
            }
        }

        public async Task<List<UserDTO>?> GetUsersAsync()
        {
            var users = await dbContext.Users.Include(_ => _.Role).ToListAsync();
            return users.Count != 0 ? mapper.Map<List<UserDTO>>(users) : null;
        }

        public async Task<UserDTO> UpdateUserAsync(UserDTO userDto)
        {
            var dbUser = await dbContext.Users.FirstOrDefaultAsync(_ => _.Id == userDto.Id);
            if (dbUser != null)
            {
                var otherUserExists = await dbContext.Users.AnyAsync(_ => _.Username == userDto.Username && _.Id != userDto.Id);
                if (otherUserExists == false)
                {
                    var userSecurityCredentials = cacheService.Get<UserSecurityCredentialsDTO>();

                    mapper.Map(userDto, dbUser);
                    dbUser.UpdatedAt = DateTime.UtcNow;
                    dbUser.UpdatedBy = userSecurityCredentials!.Id;
                    await dbContext.SaveChangesAsync();

                    dbUser = await dbContext.Users.Include(_ => _.Role).FirstOrDefaultAsync(_ => _.Id == dbUser.Id);
                    return mapper.Map<UserDTO>(dbUser);
                }
                else
                {
                    throw new ServiceException(ExceptionType.ItemAlreadyExist, details: @$"{nameof(User)} with 
                        {nameof(User.Username)} = {userDto.Username} already exists");
                }
            }
            else
            {
                throw new ServiceException(ExceptionType.ItemNotFound, details: @$"{nameof(User)} with 
                    {nameof(User.Id)} = {userDto.Id} not found");
            }
        }

        public async Task<UserSecurityCredentialsDTO> GetUserSecurityCredentialsAsync(string username)
        {
            var dbUser = await dbContext.Users.FirstOrDefaultAsync(_ => _.Username == username);
            if (dbUser != null)
            {
                return mapper.Map<UserSecurityCredentialsDTO>(dbUser);
            }
            else
            {
                throw new ServiceException(ExceptionType.ItemNotFound, details: @$"{nameof(User)} with 
                    {nameof(User.Username)} = {username} not found");
            }
        }
    }
}
