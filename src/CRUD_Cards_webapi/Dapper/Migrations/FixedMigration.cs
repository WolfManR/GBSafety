using FluentMigrator;

namespace CRUD_Cards_webapi.Dapper.Migrations;

[Migration(1)]
public class FixedMigration : Migration
{
    private const string DebetCardsTableName = "DebetCards";
    public override void Up()
    {
        Create.Table(DebetCardsTableName)
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Number").AsString(256).NotNullable()
            .WithColumn("Holder").AsString(256).NotNullable()
            .WithColumn("ExpireMonth").AsInt32().NotNullable()
            .WithColumn("ExpireYear").AsInt32().NotNullable();

    }

    public override void Down()
    {
        Delete.Table(DebetCardsTableName);
    }
}