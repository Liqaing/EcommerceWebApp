using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceWebAppProject.DB.Migrations
{
    /// <inheritdoc />
    public partial class addSessionAndPaymentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentIntentId",
                table: "OrderHeader",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "OrderHeader",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "OrderDetail",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentIntentId",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "OrderHeader");

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "OrderDetail",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
