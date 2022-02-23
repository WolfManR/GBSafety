using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity_Server.DAL;

internal sealed class IdentityDbContext : IdentityDbContext<AuthenticationUser>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {

    }
}