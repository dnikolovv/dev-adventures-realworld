using Conduit.Business.Extensions;
using Conduit.Core;
using Conduit.Data.Entities;
using Conduit.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Optional;
using System;
using System.Threading.Tasks;

namespace Conduit.Business.Services
{
    public abstract class BaseService
    {
        public BaseService(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        protected ApplicationDbContext DbContext { get; }

        protected virtual Task<Option<User, Error>> GetUserByIdOrError(string userId) =>
            GetUserById(userId.SomeNotNull())
                .WithException<User, Error>($"No user with an id of {userId} has been found.");

        protected virtual Task<Option<User, Error>> GetUserByNameOrError(string username) =>
            GetUserById(username.SomeNotNull())
                .WithException<User, Error>($"No user '{username}' has been found.");

        protected virtual Task<Option<User>> GetUserById(Option<string> userId) =>
            userId.FlatMapAsync(id =>
            GetUser(u => u.Id == id));

        protected virtual async Task<Option<User>> GetUser(Func<User, bool> predicate) =>
            (await DbContext
                .Users
                .Include(u => u.Following)
                .Include(u => u.Followers)
                .Include(u => u.Favorites)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => predicate(u))).SomeNotNull();
    }
}
