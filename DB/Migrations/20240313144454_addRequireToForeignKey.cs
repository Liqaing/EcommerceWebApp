using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceWebAppProject.DB.Migrations
{
    /// <inheritdoc />
    public partial class addRequireToForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "catId",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "catId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 2,
                column: "catId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "catId",
                value: 2);

            migrationBuilder.CreateIndex(
                name: "IX_Product_catId",
                table: "Product",
                column: "catId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Category_catId",
                table: "Product",
                column: "catId",
                principalTable: "Category",
                principalColumn: "CatId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Category_catId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_catId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "catId",
                table: "Product");
        }
    }
}
