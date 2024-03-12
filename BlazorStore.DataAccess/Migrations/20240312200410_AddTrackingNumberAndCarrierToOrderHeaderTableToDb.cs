using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackingNumberAndCarrierToOrderHeaderTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Carrier",
                table: "OrderHeaders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tracking",
                table: "OrderHeaders",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Carrier",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "Tracking",
                table: "OrderHeaders");
        }
    }
}
