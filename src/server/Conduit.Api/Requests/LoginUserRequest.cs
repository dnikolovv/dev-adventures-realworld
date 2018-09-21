using Conduit.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Conduit.Api.Requests
{
    public class LoginUserRequest
    {
        [Required]
        public CredentialsModel User { get; set; }
    }
}
