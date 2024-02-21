using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RolesToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserSecret",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserSecret",
                table: "AspNetUsers");
        }
    }
}
