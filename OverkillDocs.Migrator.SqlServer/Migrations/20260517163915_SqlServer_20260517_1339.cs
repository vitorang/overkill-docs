using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OverkillDocs.Migrator.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class SqlServer_20260517_1339 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentFragments_Users_EditedById",
                table: "DocumentFragments");

            migrationBuilder.DropIndex(
                name: "IX_DocumentFragments_EditedById",
                table: "DocumentFragments");

            migrationBuilder.DropColumn(
                name: "EditedById",
                table: "DocumentFragments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EditedById",
                table: "DocumentFragments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFragments_EditedById",
                table: "DocumentFragments",
                column: "EditedById");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentFragments_Users_EditedById",
                table: "DocumentFragments",
                column: "EditedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
