using System.Threading.Tasks;

namespace Conduit.Core.Services
{
    public interface ITagsService
    {
        Task<string[]> GetAllAsync();
    }
}
