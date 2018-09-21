using System.ComponentModel.DataAnnotations;

namespace Conduit.Core.Models
{
    public class CreateCommentModel
    {
        [Required]
        public string Body { get; set; }
    }
}
