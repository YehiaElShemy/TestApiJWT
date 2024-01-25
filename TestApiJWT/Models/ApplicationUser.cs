using Microsoft.AspNetCore.Identity;
using TestApiJWT.Models;

namespace TestApiJWT.Helper
{
    
       public class ApplicationUser : IdentityUser
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }

    }
}
