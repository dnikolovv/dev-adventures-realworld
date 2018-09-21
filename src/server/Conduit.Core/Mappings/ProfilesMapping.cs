using AutoMapper;
using Conduit.Core.Models;
using Conduit.Data.Entities;

namespace Conduit.Core.Mappings
{
    public class ProfilesMapping : Profile
    {
        public ProfilesMapping()
        {
            CreateMap<User, UserProfileModel>(MemberList.Destination)
                .ForMember(d => d.Following, opts => opts.Ignore());
        }
    }
}
