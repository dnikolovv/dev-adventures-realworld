using Conduit.Core.Services;
using Conduit.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Business.Services
{
    public class TagsService : ITagsService
    {
        public TagsService(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        protected ApplicationDbContext DbContext { get; }

        public Task<string[]> GetAllAsync() =>
            DbContext
                .Tags
                .AsNoTracking()
                .Select(t => t.Name)
                .ToArrayAsync();
    }
}
