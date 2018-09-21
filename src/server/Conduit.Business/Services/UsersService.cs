using AutoMapper;
using Conduit.Business.Extensions;
using Conduit.Core;
using Conduit.Core.Identity;
using Conduit.Core.Models;
using Conduit.Core.Services;
using Conduit.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Conduit.Business.Services
{
    public class UsersService : IUsersService
    {
        public UsersService(
            UserManager<User> userManager,
            IJwtFactory jwtFactory,
            IMapper mapper)
        {
            UserManager = userManager;
            JwtFactory = jwtFactory;
            Mapper = mapper;
        }

        protected UserManager<User> UserManager { get; }
        protected IJwtFactory JwtFactory { get; }
        protected IMapper Mapper { get; }

        public Task<Option<UserModel>> GetByIdAsync(string userId) =>
            UserManager.FindByIdAsync(userId ?? string.Empty)
                .SomeNotNull()
                .MapAsync(async user => Mapper.Map<UserModel>(user));

        public Task<Option<User, Error>> FindUserOrError(Func<User, bool> predicate, string errorIfNone) =>
            UserManager
                .Users
                .AsNoTracking()
                .Include(u => u.Following)
                .Include(u => u.Followers)
                .FirstOrDefaultAsync(u => predicate(u))
            .SomeNotNull<User, Error>(errorIfNone);

        public async Task<Option<UserModel, Error>> LoginAsync(CredentialsModel model)
        {
            var loginResult = await UserManager.FindByEmailAsync(model.Email)
                .SomeNotNull()
                .FilterAsync(user => UserManager.CheckPasswordAsync(user, model.Password));

            return loginResult.Match(
                user =>
                {
                    var result = Mapper.Map<UserModel>(user);

                    result.Token = GenerateToken(user.Id, user.Email);

                    return result.Some<UserModel, Error>();
                },
                () => Option.None<UserModel, Error>(new Error("Invalid credentials.")));
        }

        public async Task<Option<UserModel, Error>> RegisterAsync(RegisterUserModel model)
        {
            var user = Mapper.Map<User>(model);

            var creationResult = await UserManager.CreateAsync(user, model.Password)
                .SomeWhen<IdentityResult, Error>(
                    x => x.Succeeded,
                    x => x.Errors.Select(e => e.Description).ToArray());

            return await creationResult.MapAsync(async _ =>
            {
                var result = Mapper.Map<UserModel>(user);
                result.Token = GenerateToken(user.Id, user.Email);
                return result;
            });
        }

        public Task<Option<UserModel, Error>> UpdateAsync(string userId, UserModel newUser) =>
            UserManager
                .FindByIdAsync(userId)
                .SomeNotNull<User, Error>($"No user with id {userId} found.")
                .FlatMapAsync(
                    async user =>
                    {
                        var userToUpdate = Mapper.Map(newUser, user);

                        var updateResult = (await UserManager.UpdateAsync(userToUpdate))
                            .SomeWhen(
                                res => res.Succeeded,
                                res => res.Errors.Select(e => e.Description).ToArray());

                        return updateResult.Match(
                            success => Mapper.Map<UserModel>(user).Some<UserModel, Error>(),
                            errors => Option.None<UserModel, Error>(errors));
                    });

        private string GenerateToken(string userId, string email) =>
            JwtFactory.GenerateEncodedToken(userId, email, new List<Claim>());
    }
}
