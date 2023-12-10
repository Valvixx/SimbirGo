using FluentMigrator;

namespace SimbirGo.Migrations;

[Migration(1)]
public class AccountMigration : Migration
{
    public override void Up()
    {
        Create.Table("accounts")
            .WithColumn("id").AsInt32().PrimaryKey().Identity().NotNullable()
            .WithColumn("username").AsString(255)
            .WithColumn("password").AsString(255)
            .WithColumn("role").AsString(255)
            .WithColumn("email").AsString(255)
            .WithColumn("refresh_token").AsString(255)
            .WithColumn("refresh_token_expired_time").AsString(255);

        // Insert.IntoTable("accounts")
        //     .Row(new {username = "Dan", password = "Dan1234", role = "user"});

    }
    public override void Down() 
    {
        Delete.Table("accounts");
    }
}