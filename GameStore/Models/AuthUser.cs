using Microsoft.AspNetCore.Identity;

namespace GameStore.Models;

public class AuthUser : IdentityUser
{
    public DateTime BirthDate { get; set; }
}