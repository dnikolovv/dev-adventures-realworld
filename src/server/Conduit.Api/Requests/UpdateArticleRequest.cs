using Conduit.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Conduit.Api.Requests
{
    public class UpdateArticleRequest
    {
        [Required]
        public UpdateArticleModel Article { get; set; }
    }
}
