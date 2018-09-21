using AutoMapper;
using Conduit.Core.Models;
using Conduit.Data.Entities;

namespace Conduit.Core.Mappings
{
    public class ArticleCommentsMapping : Profile
    {
        public ArticleCommentsMapping()
        {
            CreateMap<CreateCommentModel, Comment>(MemberList.Source);

            CreateMap<Comment, CommentModel>(MemberList.Destination);
        }
    }
}
