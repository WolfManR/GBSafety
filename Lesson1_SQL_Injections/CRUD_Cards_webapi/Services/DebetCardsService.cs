using CRUD_Cards_webapi.EF.Entities;
using CRUD_Cards_webapi.Models;

using Thundire.Helpers;

namespace CRUD_Cards_webapi.Services;

internal sealed class DebetCardsService : IDebetCardsService
{
    private static List<DebetCardEntity> _debetCards = new();
    private static int _lastId;
    private static int IdGenerator => ++_lastId;

    public Result<int> Create(CreateDebetCardRequest cardData)
    {
        var entity = new DebetCardEntity()
        {
            Id = IdGenerator,
            Number = cardData.Number,
            Holder = cardData.Holder,
            ExpireMonth = cardData.ExpireMonth,
            ExpireYear = cardData.ExpireYear
        };
        _debetCards.Add(entity);

        return Result<int>.Ok(entity.Id);
    }

    public Result Update(int id, UpdateDebetCardRequest cardData)
    {
        if (_debetCards.FirstOrDefault(e => e.Id == id) is not { } entity) return Result.Fail();
        entity.Number = cardData.Number;
        entity.Holder = cardData.Holder;
        entity.ExpireMonth = cardData.ExpireMonth;
        entity.ExpireYear = cardData.ExpireYear;
        return Result.Ok();
    }

    public void Delete(int id)
    {
        if (_debetCards.FirstOrDefault(e => e.Id == id) is not { } entity) return;
        _debetCards.Remove(entity);
    }

    public IEnumerable<DebetCardResponse> Get()
    {
        if (_debetCards.Count <= 0) return Array.Empty<DebetCardResponse>();
        return _debetCards.Select(ToResponse).ToList();
    }

    public Result<DebetCardResponse> Get(int id)
    {
        var entity = _debetCards.FirstOrDefault(e => e.Id == id);
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