using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDeliveryBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddRatingAndLabelFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestuarntCoupon_Coupon_CouponId",
                table: "RestuarntCoupon");

            migrationBuilder.DropForeignKey(
                name: "FK_RestuarntCoupon_Restaurant_RestaurantId",
                table: "RestuarntCoupon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RestuarntCoupon",
                table: "RestuarntCoupon");

            migrationBuilder.RenameTable(
                name: "RestuarntCoupon",
                newName: "RestaurantCoupon");

            migrationBuilder.RenameIndex(
                name: "IX_RestuarntCoupon_RestaurantId",
                table: "RestaurantCoupon",
                newName: "IX_RestaurantCoupon_RestaurantId");

            migrationBuilder.RenameIndex(
                name: "IX_RestuarntCoupon_CouponId",
                table: "RestaurantCoupon",
                newName: "IX_RestaurantCoupon_CouponId");

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Restaurant",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "MenuItem",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Label",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RestaurantCoupon",
                table: "RestaurantCoupon",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantCoupon_Coupon_CouponId",
                table: "RestaurantCoupon",
                column: "CouponId",
                principalTable: "Coupon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantCoupon_Restaurant_RestaurantId",
                table: "RestaurantCoupon",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantCoupon_Coupon_CouponId",
                table: "RestaurantCoupon");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantCoupon_Restaurant_RestaurantId",
                table: "RestaurantCoupon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RestaurantCoupon",
                table: "RestaurantCoupon");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Restaurant");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "MenuItem");

            migrationBuilder.DropColumn(
                name: "Label",
                table: "Address");

            migrationBuilder.RenameTable(
                name: "RestaurantCoupon",
                newName: "RestuarntCoupon");

            migrationBuilder.RenameIndex(
                name: "IX_RestaurantCoupon_RestaurantId",
                table: "RestuarntCoupon",
                newName: "IX_RestuarntCoupon_RestaurantId");

            migrationBuilder.RenameIndex(
                name: "IX_RestaurantCoupon_CouponId",
                table: "RestuarntCoupon",
                newName: "IX_RestuarntCoupon_CouponId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RestuarntCoupon",
                table: "RestuarntCoupon",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RestuarntCoupon_Coupon_CouponId",
                table: "RestuarntCoupon",
                column: "CouponId",
                principalTable: "Coupon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RestuarntCoupon_Restaurant_RestaurantId",
                table: "RestuarntCoupon",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
