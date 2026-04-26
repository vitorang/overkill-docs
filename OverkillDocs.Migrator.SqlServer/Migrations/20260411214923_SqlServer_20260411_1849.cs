using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OverkillDocs.Migrator.SqlServer.Migrations;

/// <inheritdoc />
public partial class SqlServer_20260411_1849 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Avatar",
            table: "Users",
            type: "nvarchar(max)",
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
