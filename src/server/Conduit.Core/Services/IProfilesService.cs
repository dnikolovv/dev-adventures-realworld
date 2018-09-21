using Conduit.Core.Models;
using Optional;
using System.Threading.Tasks;

namespace Conduit.Core.Services
{
    public interface IProfilesService
    {
        Task<Option<UserProfileModel, Error>> FollowAsync(string followerId, string userToFollowUsername);

        Task<Option<UserProfileModel, Error>> UnfollowAsync(string followerId, string userToUnfollowUsername);

        Task<Option<UserProfileModel, Error>> ViewProfileAsync(string viewingUserId, string profileUsername);
    }
}
