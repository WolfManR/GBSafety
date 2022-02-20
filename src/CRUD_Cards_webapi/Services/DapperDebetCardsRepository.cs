using CRUD_Cards_webapi.Dapper;
using CRUD_Cards_webapi.Entities;
using Dapper;
using Thundire.Helpers;

namespace CRUD_Cards_webapi.Services;

internal sealed class DapperDebetCardsRepository
{
    private readonly CardsDapperDbContext _context;

    private const string TableName = "DebetCards";

    private const string sqlGetAllCards = $"SELECT * FROM {TableName}";
    private const string sqlGetById = $"SELECT * FROM {TableName} WHERE Id = @Id";
    private const string sqlInsert = $"INSERT INTO {TableName} Values (@Number, @Holder, @ExpireMonth, @ExpireYear) RETURNING id;";
    private const string sqlDelete = $"DELETE FROM {TableName} WHERE Id = @Id;";
    private const string sqlUpdate = $"UPDATE {TableName} SET Number = @Number, Holder = @Holder, ExpireMonth = @ExpireMonth, ExpireYear = @ExpireYear) WHERE Id = @Id;";

    public DapperDebetCardsRepository(CardsDapperDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Create(DebetCardEntity entity)
    {
        var returnedId = await _context.Connection.ExecuteScalarAsync<int>(sqlInsert, new
        {
            Number = entity.Number,
            Holder = entity.Holder,
            ExpireMonth = entity.ExpireMonth,
            ExpireYear = entity.ExpireYear
        });

        return Result<int>.Ok(returnedId);
    }

    public async Task<Result> Update(DebetCardEntity cardData)
    {
        var affectedRows = await _context.Connection.ExecuteAsync(sqlUpdate, new
        {
            Number = cardData.Number,
            Holder = cardData.Holder,
            ExpireMonth = cardData.ExpireMonth,
            ExpireYear = cardData.ExpireYear
        });

        return affectedRows > 0 ? Result.Ok() : Result.Fail();
    }

    public async Task Delete(int id)
    {
        await _context.Connection.ExecuteAsync(sqlDelete, new { Id = id });
    }

    public async Task<IEnumerable<DebetCardEntity>> Get()
    {
        var data = await _context.Connection.QueryAsync<DebetCardEntity>(sqlGetAllCards);
        return data;
    }

    public async Task<Result<DebetCardEntity>> Get(int id)
    {
        var entity = await _context.Connection.QueryFirstOrDefaultAsync<DebetCardEntity>(sqlGetById, new { Id = id });
        if (entity is null) return Result<DebetCardEntity>.Fail();
        return Result<DebetCardEntity>.Ok(entity);
    }
}