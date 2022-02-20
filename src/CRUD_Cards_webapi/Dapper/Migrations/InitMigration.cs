using Dapper;

namespace CRUD_Cards_webapi.Dapper.Migrations;

public class InitMigration
{
    private readonly CardsDapperDbContext _context;

    public InitMigration(CardsDapperDbContext context)
    {
        _context = context;
    }

    public async Task Up()
    {
        await _context.Connection.ExecuteAsync(@"CREATE TABLE ""DebetCards"" (
        ""Id"" serial NOT NULL,PRIMARY KEY(""Id""),
        ""Number"" text NOT NULL,
        ""Holder"" text NOT NULL,
        ""ExpireMonth"" integer NOT NULL,
        ""ExpireYear"" integer NOT NULL
            );");
    }
}