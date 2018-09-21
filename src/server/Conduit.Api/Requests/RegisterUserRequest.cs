using Conduit.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Conduit.Api.Requests
{
    public class RegisterUserRequest
    {
        [Required]
        public RegisterUserModel User { get; set; }
    }
}
