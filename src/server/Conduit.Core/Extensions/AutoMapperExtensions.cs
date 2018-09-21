using AutoMapper;

namespace Conduit.Core.Extensions
{
    public static class AutoMapperExtensions
    {
        public static void IgnoreNullSourceProperties<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mapping) =>
            mapping.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
