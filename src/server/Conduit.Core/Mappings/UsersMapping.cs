using AutoMapper;
using Conduit.Core.Extensions;
using Conduit.Core.Models;
using Conduit.Data.Entities;

namespace Conduit.Core.Mappings
{
    public class UsersMapping : Profile
    {
        public UsersMapping()
        {
            CreateMap<User, UserModel>(MemberList.Destination)
                .ForMember(d => d.Token, opts => opts.Ignore());

            CreateMap<UserModel, User>(MemberList.Source)
                .ForSourceMember(s => s.Token, opts => opts.Ignore())
                .IgnoreNullSourceProperties();

            CreateMap<RegisterUserModel, User>(MemberList.Source)
                .ForMember(d => d.UserName, opts => opts.MapFrom(s => s.Username))
                .ForSourceMember(s => s.Password, opts => opts.Ignore())
                .IgnoreNullSourceProperties();
        }
    }
}
