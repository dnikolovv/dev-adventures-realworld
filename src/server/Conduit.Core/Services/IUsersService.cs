using Conduit.Core.Models;
using Optional;
using System.Threading.Tasks;

namespace Conduit.Core.Services
{
    public interface IUsersService
    {
        Task<Option<UserModel, Error>> UpdateAsync(string userId, UserModel newUser);

        Task<Option<UserModel, Error>> LoginAsync(CredentialsModel model);

        Task<Option<UserModel, Error>> RegisterAsync(RegisterUserModel model);

        Task<Option<UserModel>> GetByIdAsync(string userId);
    }
}
