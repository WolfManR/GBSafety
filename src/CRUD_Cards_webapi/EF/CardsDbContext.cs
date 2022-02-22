using CRUD_Cards_webapi.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRUD_Cards_webapi.EF;

internal sealed class CardsDbContext : DbContext
{
    public CardsDbContext(DbContextOptions<CardsDbContext> options) : base(options)
    {
    }

#nullable disable
    public DbSet<DebetCardEntity> DebetCards { get; init; }
#nullable restore
}