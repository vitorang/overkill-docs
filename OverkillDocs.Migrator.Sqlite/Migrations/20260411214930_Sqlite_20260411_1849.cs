using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OverkillDocs.Migrator.Sqlite.Migrations;

/// <inheritdoc />
public partial class Sqlite_20260411_1849 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Avatar",
            table: "Users",
            type: "TEXT",
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Avatar",
            table: "Users");
    }
}
