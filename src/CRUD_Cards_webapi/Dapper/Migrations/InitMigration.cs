using Dapper;

namespace CRUD_Cards_webapi.Dapper.Migrations;

public class InitMigration
{
    private readonly CardsDapperDbContext _context;

    private const string DebetCardsTableName = "DebetCards";

    public InitMigration(CardsDapperDbContext context)
    {
        _context = context;
    }

    public async Task Up()
    {
        if (await TableExist(DebetCardsTableName)) return;
        await _context.Connection.ExecuteAsync($@"CREATE TABLE ""{DebetCardsTableName}"" (
        ""Id"" serial NOT NULL,PRIMARY KEY(""Id""),
        ""Number"" text NOT NULL,
        ""Holder"" text NOT NULL,
        ""ExpireMonth"" integer NOT NULL,
        ""ExpireYear"" integer NOT NULL
            );");
    }

    private async Task<bool> TableExist(string tableName)
    {
        var result = await _context.Connection.QuerySingleAsync<bool>($@"SELECT EXISTS (
    SELECT FROM 
        information_schema.tables 
    WHERE 
        table_schema LIKE 'public' AND 
        table_type LIKE 'BASE TABLE' AND
        table_name = '{tableName}'
    );");

        return result;
    }
}