using Conduit.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Conduit.Api.Requests
{
    public class UpdateUserRequest
    {
        [Required]
        public UserModel User { get; set; }
    }
}
