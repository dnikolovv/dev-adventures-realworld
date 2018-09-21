namespace Conduit.Data.Entities
{
    public class FollowedUser
    {
        public string FollowerId { get; set; }

        public string FollowingId { get; set; }

        public User Follower { get; set; }

        public User Following { get; set; }
    }
}
