using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDeliveryBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantRegistrationRequest_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RestaurantRegistrationRequest",
                table: "RestaurantRegistrationRequest");

            migrationBuilder.RenameTable(
                name: "RestaurantRegistrationRequest",
                newName: "RestaurantRegistrationRequests");

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantName",
                table: "RestaurantRegistrationRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "RestaurantRegistrationRequests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "RestaurantRegistrationRequests",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RestaurantRegistrationRequests",
                table: "RestaurantRegistrationRequests",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RestaurantRegistrationRequests",
                table: "RestaurantRegistrationRequests");

            migrationBuilder.RenameTable(
                name: "RestaurantRegistrationRequests",
                newName: "RestaurantRegistrationRequest");

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantName",
                table: "RestaurantRegistrationRequest",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "RestaurantRegistrationRequest",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "RestaurantRegistrationRequest",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RestaurantRegistrationRequest",
                table: "RestaurantRegistrationRequest",
                column: "Id");
        }
    }
}
