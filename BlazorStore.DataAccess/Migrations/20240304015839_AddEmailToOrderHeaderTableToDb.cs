using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailToOrderHeaderTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "OrderHeaders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "OrderHeaders");
        }
    }
}
