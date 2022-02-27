using Microsoft.AspNetCore.Identity;

namespace Identity_Server.DAL;

internal sealed class AuthenticationUser : IdentityUser
{
    public string? Token { get; set; }
    public DateTime TokenExpires { get; set; }
}