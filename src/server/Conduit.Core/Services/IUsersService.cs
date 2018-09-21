using Conduit.Core.Models;
using Conduit.Data.Entities;
using Optional;
using System;
using System.Threading.Tasks;

namespace Conduit.Core.Services
{
    public interface IUsersService
    {
        Task<Option<UserModel, Error>> UpdateAsync(string userId, UserModel newUser);

        Task<Option<UserModel, Error>> LoginAsync(CredentialsModel model);

        Task<Option<UserModel, Error>> RegisterAsync(RegisterUserModel model);

        Task<Option<UserModel>> GetByIdAsync(string userId);

        Task<Option<User, Error>> FindUserOrError(Func<User, bool> predicate, string errorIfNone);
    }
}
