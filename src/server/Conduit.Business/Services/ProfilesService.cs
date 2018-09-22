using AutoMapper;
using Conduit.Business.Extensions;
using Conduit.Core;
using Conduit.Core.Models;
using Conduit.Core.Services;
using Conduit.Data.Entities;
using Conduit.Data.EntityFramework;
using Optional;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Business.Services
{
    public class ProfilesService : BaseService, IProfilesService
    {
        public ProfilesService(
            IUsersService usersService,
            ApplicationDbContext dbContext,
            IMapper mapper)
            : base(dbContext)
        {
            UsersService = usersService;
            Mapper = mapper;
        }

        protected IMapper Mapper { get; }
        protected IUsersService UsersService { get; }

        public Task<Option<UserProfileModel, Error>> FollowAsync(string followerId, string userToFollowUsername) =>
            GetUserByIdOrError(followerId).FlatMapAsync(user =>
            GetUserByNameOrError(userToFollowUsername)
                .FilterAsync(async u => u.Id != followerId, "A user cannot follow himself.")
                .FilterAsync(async u => user.Following.All(fu => fu.FollowingId != u.Id), "You are already following this user")
                .FlatMapAsync(async userToFollow =>
                {
                    DbContext.FollowedUsers.Add(new FollowedUser
                    {
                        FollowerId = followerId,
                        FollowingId = userToFollow.Id
                    });

                    await DbContext.SaveChangesAsync();

                    return await ViewProfileAsync(followerId.Some(), userToFollow.UserName);
                }));

        public Task<Option<UserProfileModel, Error>> UnfollowAsync(string followerId, string userToUnfollowUsername) =>
            GetUserByIdOrError(followerId).FlatMapAsync(user =>
            GetUserByNameOrError(userToUnfollowUsername)
                .FilterAsync(async u => u.Id != followerId, "A user cannot unfollow himself.")
                .FilterAsync(async u => user.Following.Any(fu => fu.FollowingId == u.Id), "You cannot unfollow users that you aren't following.")
                .FlatMapAsync(async userToUnfollow =>
                {
                    var relationToRemove = user.Following.First(f => f.FollowingId == userToUnfollow.Id);

                    DbContext.FollowedUsers.Remove(relationToRemove);

                    await DbContext.SaveChangesAsync();

                    return await ViewProfileAsync(followerId.Some(), userToUnfollow.UserName);
                }));

        public Task<Option<UserProfileModel, Error>> ViewProfileAsync(Option<string> viewingUserId, string profileUsername) =>
            GetUserByNameOrError(profileUsername)
                .MapAsync(async userToView =>
                {
                    var profile = Mapper.Map<UserProfileModel>(userToView);

                    (await GetUserById(viewingUserId)).MatchSome(viewingUser =>
                    {
                        // If the user that is viewing the profile has the profile owner in his followers
                        profile.Following = viewingUser
                            .Following?
                            .Any(f => f.FollowingId == userToView.Id) ?? false;
                    });

                    return profile;
                });
    }
}