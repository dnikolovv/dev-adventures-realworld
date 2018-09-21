using Conduit.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Conduit.Api.Requests
{
    public class CreateCommentRequest
    {
        [Required]
        public CreateCommentModel Comment { get; set; }
    }
}
