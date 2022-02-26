using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity_Server.DAL;

internal sealed class AuthenticationDbContext : IdentityDbContext<AuthenticationUser>
{
    public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : base(options)
    {

    }
}