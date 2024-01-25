using System.ComponentModel.DataAnnotations;

namespace TestApiJWT.Models
{
    public class TokenRequestModel
    {
        [Required, StringLength(100)]
        public string Email { get; set; }
        [Required, StringLength(100)]
        public string Password { get; set; }
    }
}
