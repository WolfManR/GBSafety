using CRUD_Cards_webapi.EF;
using CRUD_Cards_webapi.Entities;
using Microsoft.EntityFrameworkCore;
using Thundire.Helpers;

namespace CRUD_Cards_webapi.Services;

internal sealed class EFCoreDebetCardsRepository
{
    private readonly CardsDbContext _context;

    public EFCoreDebetCardsRepository(CardsDbContext context) => _context = context;

    public async Task<Result<int>> Create(DebetCardEntity entity)
    {
        await _context.DebetCards.AddAsync(entity);
        await _context.SaveChangesAsync();

        return Result<int>.Ok(entity.Id);
    }

    public async Task<Result> Update(DebetCardEntity cardData)
    {
        var entry = _context.Entry(cardData);

        entry.State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task Delete(int id)
    {
        var entity = await _context.DebetCards.FirstOrDefaultAsync(e => e.Id == id);
        if (entity is null) return;
        _context.DebetCards.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<DebetCardEntity>> Get()
    {
        var data = await _context.DebetCards.ToArrayAsync();
        return data;
    }

    public async Task<Result<DebetCardEntity>> Get(int id)
    {
        var entity = await _context.DebetCards.FirstOrDefaultAsync(e => e.Id == id);
        if (entity is null) return Result<DebetCardEntity>.Fail();
        return Result<DebetCardEntity>.Ok(entity);
    }
}