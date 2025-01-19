using Microsoft.AspNetCore.Identity;

namespace Api.Model
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
    }
}
