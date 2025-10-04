using Compago.Data;
using Compago.Domain;
using Compago.Service;
using Compago.Service.CustomeException;
using Compago.Test.Helper;
using Compago.Test.Helper.Domain;
using Microsoft.EntityFrameworkCore;

namespace Compago.Test.Service
{
    public class UserServiceTest : ServiceTest
    {
        public class AddUser
        {
            [Fact]
            public async Task PasswordMissing()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var userService = new UserService(dbContext, cacheService, _mapper);

                var user = UserHelper.New(password: null);

                // Act
                var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                    userService.AddUserAsync(user));

                // Assert
                Assert.Equal(ExceptionType.InvalidRequest, exception.ExceptionType);
                Assert.Contains(nameof(User), exception.Message);
                Assert.Contains(nameof(UserDTO.Password), exception.Message);
            }

            [Fact]
            public async Task UsernameExists()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var userService = new UserService(dbContext, cacheService, _mapper);

                var username = "TestUser";
                var userDb = UserHelper.NewDb(username: username);
                await dbContext.Users.AddAsync(userDb);
                await dbContext.SaveChangesAsync();

                var user = UserHelper.New(username: username);

                // Act
                var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                    userService.AddUserAsync(user));

                // Assert
                Assert.Equal(ExceptionType.ItemAlreadyExist, exception.ExceptionType);
                Assert.Contains(nameof(User), exception.Message);
                Assert.Contains(nameof(User.Username), exception.Message);
                Assert.Contains(username, exception.Message);
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var userService = new UserService(dbContext, cacheService, _mapper);

                var user = UserHelper.New(id: null);

                // Act
                var result = await userService.AddUserAsync(user);

                // Assert
                var dbUser = await dbContext.Users.FirstOrDefaultAsync(_ => _.Id == 1);
                Assert.NotNull(dbUser);
                Assert.Equal(dbUser.CreatedBy, _cacheUserId);
                Assert.True(dbUser.CreatedAt > DateTime.UtcNow.AddMinutes(-1) && dbUser.CreatedAt < DateTime.UtcNow.AddMinutes(1));
            }
        }

        public class GetUsers
        {
            [Fact]
            public async Task EmptyResult()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var userService = new UserService(dbContext, cacheService, _mapper);

                // Act
                var result = await userService.GetUsersAsync();

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var userService = new UserService(dbContext, cacheService, _mapper);

                var user1 = UserHelper.NewDb(id: 1, username: "User1");
                var user2 = UserHelper.NewDb(id: 2, username: "User2");
                await dbContext.Users.AddAsync(user1);
                await dbContext.Users.AddAsync(user2);
                await dbContext.SaveChangesAsync();

                // Act
                var result = await userService.GetUsersAsync();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                Assert.Contains(result, _ => _.Id == user1.Id && _.Username == user1.Username);
                Assert.Contains(result, _ => _.Id == user2.Id && _.Username == user2.Username);
            }
        }

        public class GetUser
        {
            [Fact]
            public async Task UserNotFound()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var userService = new UserService(dbContext, cacheService, _mapper);

                var userDb = UserHelper.NewDb();
                await dbContext.Users.AddAsync(userDb);
                await dbContext.SaveChangesAsync();

                var requestId = userDb.Id + 1;

                // Act
                var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                    userService.GetUserAsync(requestId));

                // Assert
                Assert.Equal(ExceptionType.ItemNotFound, exception.ExceptionType);
                Assert.Contains(nameof(User), exception.Message);
                Assert.Contains(nameof(User.Id), exception.Message);
                Assert.Contains(requestId.ToString(), exception.Message);
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var userService = new UserService(dbContext, cacheService, _mapper);

                var userDb1 = UserHelper.NewDb(id: 1, username: "user1");
                var userDb2 = UserHelper.NewDb(id: 2, username: "user2");
                await dbContext.Users.AddRangeAsync(userDb1, userDb2);
                await dbContext.SaveChangesAsync();

                // Act
                var result = await userService.GetUserAsync(userDb2.Id);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(userDb2.Id, result.Id);
                Assert.Equal(userDb2.Username, result.Username);
                Assert.Equal(userDb2.RoleId, (short)result.RoleId);
            }
        }

        public class UpdateUser
        {
            [Fact]
            public async Task UserNotFound()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var userService = new UserService(dbContext, cacheService, _mapper);

                var userDb = UserHelper.NewDb();
                await dbContext.Users.AddAsync(userDb);
                await dbContext.SaveChangesAsync();

                var user = UserHelper.New(id: userDb.Id + 1);

                // Act
                var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                    userService.UpdateUserAsync(user));

                // Assert
                Assert.Equal(ExceptionType.ItemNotFound, exception.ExceptionType);
                Assert.Contains(nameof(User), exception.Message);
                Assert.Contains(nameof(User.Id), exception.Message);
                Assert.Contains(nameof(user.Id), exception.Message);
            }

            [Fact]
            public async Task UsernameExists()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var userService = new UserService(dbContext, cacheService, _mapper);

                var userDb1 = UserHelper.NewDb(id: 1, username: "user1");
                var userDb2 = UserHelper.NewDb(id: 2, username: "user2");
                await dbContext.Users.AddRangeAsync(userDb1, userDb2);
                await dbContext.SaveChangesAsync();

                var user = UserHelper.New(id: userDb1.Id, username: userDb2.Username);

                // Act
                var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                    userService.UpdateUserAsync(user));

                // Assert
                Assert.Equal(ExceptionType.ItemAlreadyExist, exception.ExceptionType);
                Assert.Contains(nameof(User), exception.Message);
                Assert.Contains(nameof(User.Username), exception.Message);
                Assert.Contains(nameof(user.Username), exception.Message);
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var userService = new UserService(dbContext, cacheService, _mapper);

                var userDb1 = UserHelper.NewDb(id: 1, username: "user1");
                var userDb2 = UserHelper.NewDb(id: 2, username: "user2", roleId: Compago.Common.Role.User);
                await dbContext.Users.AddRangeAsync(userDb1, userDb2);
                await dbContext.SaveChangesAsync();

                var newUsername = "newUsername";
                var newRole = Compago.Common.Role.Admin;
                var user = UserHelper.New(id: userDb2.Id, username: newUsername, roleId: newRole);

                // Act
                var result = await userService.UpdateUserAsync(user);

                // Assert
                var updateDbUser = await dbContext.Users.FirstOrDefaultAsync(_ => _.Id == userDb2.Id);
                Assert.NotNull(result);
                Assert.Equal(newUsername, result.Username);
                Assert.Equal(newRole, result.RoleId);

                Assert.NotNull(updateDbUser);
                Assert.Equal(newUsername, updateDbUser.Username);
                Assert.Equal((short)newRole, updateDbUser.RoleId);

                Assert.Equal(updateDbUser.UpdatedBy, _cacheUserId);
                Assert.True(updateDbUser.UpdatedAt > DateTime.UtcNow.AddMinutes(-1) && updateDbUser.UpdatedAt < DateTime.UtcNow.AddMinutes(1));
            }
        }

        public class DeleteUser
        {
            [Fact]
            public async Task UserNotFound()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var userService = new UserService(dbContext, cacheService, _mapper);

                var userDb = UserHelper.NewDb();
                await dbContext.Users.AddAsync(userDb);
                await dbContext.SaveChangesAsync();

                var requestId = userDb.Id + 1;

                // Act
                var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                    userService.DeleteUserAsync(requestId));

                // Assert
                Assert.Equal(ExceptionType.ItemNotFound, exception.ExceptionType);
                Assert.Contains(nameof(User), exception.Message);
                Assert.Contains(nameof(User.Id), exception.Message);
                Assert.Contains(requestId.ToString(), exception.Message);
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var userService = new UserService(dbContext, cacheService, _mapper);

                var userDb1 = UserHelper.NewDb(id: 1, username: "user1");
                var userDb2 = UserHelper.NewDb(id: 2, username: "user2");
                await dbContext.Users.AddRangeAsync(userDb1, userDb2);
                await dbContext.SaveChangesAsync();

                // Act
                await userService.DeleteUserAsync(userDb2.Id);

                // Assert
                var updateDbUser1 = await dbContext.Users.FirstOrDefaultAsync(_ => _.Id == userDb1.Id);
                var updateDbUser2 = await dbContext.Users.FirstOrDefaultAsync(_ => _.Id == userDb2.Id);
                Assert.NotNull(updateDbUser1);
                Assert.Null(updateDbUser2);
            }
        }

        public class GetUserSecurityCredentials
        {
            [Fact]
            public async Task UserNotFound()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var userService = new UserService(dbContext, cacheService, _mapper);

                var userDb = UserHelper.NewDb();
                await dbContext.Users.AddAsync(userDb);
                await dbContext.SaveChangesAsync();

                var requesUsername = userDb.Username + "1";

                // Act
                var exception = await Assert.ThrowsAsync<ServiceException>(() =>
                    userService.GetUserSecurityCredentialsAsync(requesUsername));

                // Assert
                Assert.Equal(ExceptionType.ItemNotFound, exception.ExceptionType);
                Assert.Contains(nameof(User), exception.Message);
                Assert.Contains(nameof(User.Username), exception.Message);
                Assert.Contains(requesUsername.ToString(), exception.Message);
            }

            [Fact]
            public async Task Success()
            {
                // Arrange
                var dbContext = await DatabaseHelper.GetContextAsync();
                var cacheService = GetCacheService();
                var userService = new UserService(dbContext, cacheService, _mapper);

                var userDb1 = UserHelper.NewDb(id: 1, username: "user1");
                var userDb2 = UserHelper.NewDb(id: 2, username: "user2");
                await dbContext.Users.AddRangeAsync(userDb1, userDb2);
                await dbContext.SaveChangesAsync();

                // Act
                var result = await userService.GetUserSecurityCredentialsAsync(userDb2.Username);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(userDb2.RoleId, (short)result.RoleId);
                Assert.Equal(userDb2.PasswordHash, result.PasswordHash);
                Assert.Equal(userDb2.PasswordHashSalt, result.PasswordHashSalt);
            }
        }
    }
}
