using AutoMapper;
using Conduit.Core.Extensions;
using Conduit.Core.Models;
using Conduit.Data.Entities;
using System.Linq;

namespace Conduit.Core.Mappings
{
    public class ArticlesMapping : Profile
    {
        public ArticlesMapping()
        {
            CreateMap<CreateArticleModel, Article>(MemberList.Source)
                .ForMember(d => d.TagList, opts => opts.Ignore())
                .ForMember(d => d.Slug, opts => opts.MapFrom(s => MakeSlug(s.Title)));

            CreateMap<UpdateArticleModel, Article>(MemberList.Source)
                .ForMember(d => d.Slug, opts => opts.MapFrom(s => MakeSlug(s.Title)))
                .IgnoreNullSourceProperties();

            CreateMap<Article, ArticleModel>(MemberList.Destination)
                .ForMember(d => d.TagList, opts => opts.MapFrom(s => s.TagList.Select(at => at.Tag.Name)));

            CreateMap<ArticleTag, string>()
                .ConvertUsing(s => s.Tag.Name);

            CreateMap<ArticleTag, string>()
                .ProjectUsing(s => s.Tag.Name);
        }

        private static string MakeSlug(string title) =>
            title.ToLower().Replace(' ', '-');
    }
}
