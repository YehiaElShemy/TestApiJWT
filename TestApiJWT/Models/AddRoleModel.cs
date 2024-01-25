using System.ComponentModel.DataAnnotations;

namespace TestApiJWT.Models
{
    public class AddRoleModel   
    {
      
        [Required, StringLength(100)]
        public string UserId { get; set; }
        [Required, StringLength(100)]
        public string Role { get; set; }
    }
}
