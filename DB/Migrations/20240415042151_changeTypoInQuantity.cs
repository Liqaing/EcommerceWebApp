using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceWebAppProject.DB.Migrations
{
    /// <inheritdoc />
    public partial class changeTypoInQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "qauntity",
                table: "ShoppingCart",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "Qauntity",
                table: "Product",
                newName: "Quantity");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "OrderHeader",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "OrderHeader");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "ShoppingCart",
                newName: "qauntity");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Product",
                newName: "Qauntity");
        }
    }
}
