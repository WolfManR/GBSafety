using CRUD_Cards_webapi.EF;
using CRUD_Cards_webapi.Entities;
using CRUD_Cards_webapi.Models;

using Thundire.Helpers;

namespace CRUD_Cards_webapi.Services;

internal sealed class EFCoreDebetCardsService
{
    private readonly EFCoreDebetCardsRepository _efCoreDebetCardsRepository;

    public EFCoreDebetCardsService(EFCoreDebetCardsRepository efCoreDebetCardsRepository)
    {
        _efCoreDebetCardsRepository = efCoreDebetCardsRepository;
    }

    public async Task<Result<int>> Create(CreateDebetCardRequest cardData)
    {
        var entity = new DebetCardEntity()
        {
            Number = cardData.Number,
            Holder = cardData.Holder,
            ExpireMonth = cardData.ExpireMonth,
            ExpireYear = cardData.ExpireYear
        };
        var result = await _efCoreDebetCardsRepository.Create(entity);

        if(result.IsSuccess) return Result<int>.Ok(entity.Id);

        return Result<int>.Fail(new FailureDescription("Fail to create debet card", ""));
    }

    public async Task<Result> Update(int id, UpdateDebetCardRequest cardData)
    {
        var result = await _efCoreDebetCardsRepository.Update(new DebetCardEntity()
        {
            Id = id,
            Holder = cardData.Holder,
            Number = cardData.Number,
            ExpireMonth = cardData.ExpireMonth,
            ExpireYear = cardData.ExpireYear
        });

        if (result.IsSuccess) return Result.Ok();

        if (result is FailureResult failureResult)
        {
            // Log
        }

        return Result.Fail(new FailureDescription("Fail to update debet card entry", ""));
    }

    public async Task Delete(int id)
    {
        await _efCoreDebetCardsRepository.Delete(id);
    }

    public async Task<IEnumerable<DebetCardResponse>> Get()
    {
        var data = await _efCoreDebetCardsRepository.Get();
        return data.Select(ToResponse);
    }

    public async Task<Result<DebetCardResponse>> Get(int id)
    {
        var result = await _efCoreDebetCardsRepository.Get(id);
        if (!result.IsSuccess) return Result<DebetCardResponse>.Fail();
        var response = ToResponse(result.GetResult());
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