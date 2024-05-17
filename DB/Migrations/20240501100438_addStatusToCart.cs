using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceWebAppProject.DB.Migrations
{
    /// <inheritdoc />
    public partial class addStatusToCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "shoppingCartStatus",
                table: "ShoppingCart",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "shoppingCartStatus",
                table: "ShoppingCart");
        }
    }
}
