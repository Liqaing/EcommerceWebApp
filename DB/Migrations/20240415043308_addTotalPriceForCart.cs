using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceWebAppProject.DB.Migrations
{
    /// <inheritdoc />
    public partial class addTotalPriceForCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "totalPrice",
                table: "ShoppingCart",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "totalPrice",
                table: "ShoppingCart");
        }
    }
}
