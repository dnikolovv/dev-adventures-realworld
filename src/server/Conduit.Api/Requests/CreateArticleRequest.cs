using Conduit.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Conduit.Api.Requests
{
    public class CreateArticleRequest
    {
        [Required]
        public CreateArticleModel Article { get; set; }
    }
}
