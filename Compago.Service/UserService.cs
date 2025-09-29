using AutoMapper;
using Compago.Data;
using Compago.Domain;
using Compago.Service.CustomeException;
using Microsoft.EntityFrameworkCore;

namespace Compago.Service
{
    public interface IUserService
    {
        Task<UserDTO> AddUserAsync(UserDTO userDto);
        Task<List<UserDTO>> GetUsersAsync();
        Task<UserDTO> GetUserAsync(int userId);
        Task<UserDTO> UpdateUserAsync(UserDTO userDto);
        Task DeleteUserAsync(int userId);
    }

    public class UserService(
        CompagoDbContext dbContext,
        IMapper mapper) : IUserService
    {
        public async Task<UserDTO> AddUserAsync(UserDTO userDto)
        {
            var userExists = await dbContext.Users.AnyAsync(_ => _.Username == userDto.Username);
            if (userExists == false)
            {
                var roleExists = await dbContext.Roles.AnyAsync(_ => _.Id == userDto.RoleId);
                if (roleExists == true)
                {
                    var dbUser = mapper.Map<User>(userDto);
                    await dbContext.Users.AddAsync(dbUser);
                    await dbContext.SaveChangesAsync();

                    dbUser = await dbContext.Users.Include(_ => _.Role).FirstOrDefaultAsync(_ => _.Id == dbUser.Id);
                    return mapper.Map<UserDTO>(dbUser);
                }
                else
                {
                    throw new ServiceException(ExceptionType.ItemNotFound, details: @$"{nameof(Role)} with 
                        {nameof(Role.Id)} = {userDto.RoleId} not found");
                }
            } 
            else
            {
                throw new ServiceException(ExceptionType.ItemAlreadyExist, details: @$"{nameof(User)} with 
                    {nameof(User.Username)} = {userDto.Username} already exists");
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

        public async Task<List<UserDTO>> GetUsersAsync()
        {
            var users = await dbContext.Users.Include(_ => _.Role).ToListAsync();
            return mapper.Map<List<UserDTO>>(users);
        }

        public async Task<UserDTO> UpdateUserAsync(UserDTO userDto)
        {
            var dbUser = await dbContext.Users.FirstOrDefaultAsync(_ => _.Id == userDto.Id);
            if (dbUser != null)
            {
                var otherUserExists = await dbContext.Users.AnyAsync(_ => _.Username == userDto.Username && _.Id != userDto.Id);
                if (otherUserExists == false)
                {
                    var roleExists = await dbContext.Roles.AnyAsync(_ => _.Id == userDto.RoleId);
                    if (roleExists == true)
                    {
                        mapper.Map(userDto, dbUser);
                        await dbContext.SaveChangesAsync();

                        dbUser = await dbContext.Users.Include(_ => _.Role).FirstOrDefaultAsync(_ => _.Id == dbUser.Id);
                        return mapper.Map<UserDTO>(dbUser);
                    }
                    else
                    {
                        throw new ServiceException(ExceptionType.ItemNotFound, details: @$"{nameof(Role)} with 
                            {nameof(Role.Id)} = {userDto.RoleId} not found");
                    }
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
    }
}
