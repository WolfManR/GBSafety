using FluentMigrator;

namespace CRUD_Cards_webapi.Dapper.Migrations;

[Migration(14218022022)]
public class InitMigration : Migration
{
    public override void Up()
    {
        Create.Table("DebetCards")
            .WithColumn("Id").AsGuid().PrimaryKey().Identity()
            .WithColumn("Number").AsString()
            .WithColumn("Holder").AsString()
            .WithColumn("ExpireMonth").AsInt32()
            .WithColumn("ExpireYear").AsInt32();
    }

    public override void Down()
    {
        Delete.Table("DebetCards");
    }
}