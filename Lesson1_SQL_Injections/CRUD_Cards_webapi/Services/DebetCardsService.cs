using CRUD_Cards_webapi.EF;
using CRUD_Cards_webapi.EF.Entities;
using CRUD_Cards_webapi.Models;

using Microsoft.EntityFrameworkCore;

using Thundire.Helpers;

namespace CRUD_Cards_webapi.Services;

internal sealed class DebetCardsService : IDebetCardsService
{
    private readonly CardsDbContext _context;

    public DebetCardsService(CardsDbContext context) => _context = context;

    public async Task<Result<int>> Create(CreateDebetCardRequest cardData)
    {
        var entity = new DebetCardEntity()
        {
            Number = cardData.Number,
            Holder = cardData.Holder,
            ExpireMonth = cardData.ExpireMonth,
            ExpireYear = cardData.ExpireYear
        };
        await _context.DebetCards.AddAsync(entity);
        await _context.SaveChangesAsync();

        return Result<int>.Ok(entity.Id);
    }

    public async Task<Result> Update(int id, UpdateDebetCardRequest cardData)
    {
        var entity = await _context.DebetCards.FirstOrDefaultAsync(e => e.Id == id);
        if (entity is null) return Result.Fail();

        entity.Number = cardData.Number;
        entity.Holder = cardData.Holder;
        entity.ExpireMonth = cardData.ExpireMonth;
        entity.ExpireYear = cardData.ExpireYear;

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

    public async Task<IEnumerable<DebetCardResponse>> Get()
    {
        var data = await _context.DebetCards.ToArrayAsync();
        return data.Select(ToResponse);
    }

    public async Task<Result<DebetCardResponse>> Get(int id)
    {
        var entity = await _context.DebetCards.FirstOrDefaultAsync(e => e.Id == id);
        if (entity is null) return Result<DebetCardResponse>.Fail();
        var response = ToResponse(entity);
        return Result<DebetCardResponse>.Ok(response);
    }

    private static DebetCardResponse ToResponse(DebetCardEntity entity) => new DebetCardResponse()
    {
        Id = entity.Id,
        Number = entity.Number,
        Holder = entity.Holder,
        ExpireMonth = entity.ExpireMonth,
        ExpireYear = entity.ExpireYear
    };
}