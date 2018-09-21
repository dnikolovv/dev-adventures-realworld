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
    public class ProfilesService : IProfilesService
    {
        public ProfilesService(
            IUsersService usersService,
            ApplicationDbContext dbContext,
            IMapper mapper)
        {
            UsersService = usersService;
            DbContext = dbContext;
            Mapper = mapper;
        }

        protected ApplicationDbContext DbContext { get; }
        protected IMapper Mapper { get; }
        protected IUsersService UsersService { get; }

        public Task<Option<UserProfileModel, Error>> FollowAsync(string followerId, string userToFollowUsername) =>
            UsersService
                .FindUserOrError(u => u.Id == followerId, $"A user with the id of {followerId} was not found.")
                .FlatMapAsync(
                    user => UsersService
                        .FindUserOrError(u => u.UserName == userToFollowUsername, $"Could not find user {userToFollowUsername}.")
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

                            return await ViewProfileAsync(followerId, userToFollow.UserName);
                        }));

        public Task<Option<UserProfileModel, Error>> UnfollowAsync(string followerId, string userToUnfollowUsername) =>
            UsersService
                .FindUserOrError(u => u.Id == followerId, $"A user with the id of {followerId} was not found.")
                .FlatMapAsync(
                    user => UsersService
                        .FindUserOrError(u => u.UserName == userToUnfollowUsername, $"Could not find user {userToUnfollowUsername}.")
                        .FilterAsync(async u => u.Id != followerId, "A user cannot unfollow himself.")
                        .FilterAsync(async u => user.Following.Any(fu => fu.FollowingId == u.Id), "You cannot unfollow users that you aren't following.")
                        .FlatMapAsync(async userToUnfollow =>
                        {
                            var relationToRemove = user.Following.First(f => f.FollowingId == userToUnfollow.Id);

                            DbContext.FollowedUsers.Remove(relationToRemove);

                            await DbContext.SaveChangesAsync();

                            return await ViewProfileAsync(followerId, userToUnfollow.UserName);
                        }));

        public Task<Option<UserProfileModel, Error>> ViewProfileAsync(string viewingUserId, string profileUsername) =>
            UsersService
                .FindUserOrError(u => u.Id == viewingUserId, "The viewing user id hasn't been found in the database.")
                .FlatMapAsync(
                    viewingUser => UsersService
                        .FindUserOrError(u => u.UserName == profileUsername, $"No user with the name '{profileUsername}' has been found.")
                        .MapAsync(async userToView =>
                        {
                            var profile = Mapper.Map<UserProfileModel>(userToView);

                            // If the user that is viewing the profile has the profile owner in his followers
                            profile.Following = viewingUser
                                .Following?
                                .Any(f => f.FollowingId == userToView.Id) ?? false;

                            return profile;
                        }));
    }
}