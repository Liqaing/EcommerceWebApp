using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceWebAppProject.DB.Migrations
{
    /// <inheritdoc />
    public partial class AddKeyToEmpFromOrderHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ArrivalDate",
                table: "OrderHeader",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledEmpId",
                table: "OrderHeader",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryEmpId",
                table: "OrderHeader",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cancelBy",
                table: "OrderHeader",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "deliveryEmpName",
                table: "OrderHeader",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeader_CancelledEmpId",
                table: "OrderHeader",
                column: "CancelledEmpId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeader_DeliveryEmpId",
                table: "OrderHeader",
                column: "DeliveryEmpId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeader_AspNetUsers_CancelledEmpId",
                table: "OrderHeader",
                column: "CancelledEmpId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeader_AspNetUsers_DeliveryEmpId",
                table: "OrderHeader",
                column: "DeliveryEmpId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeader_AspNetUsers_CancelledEmpId",
                table: "OrderHeader");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeader_AspNetUsers_DeliveryEmpId",
                table: "OrderHeader");

            migrationBuilder.DropIndex(
                name: "IX_OrderHeader_CancelledEmpId",
                table: "OrderHeader");

            migrationBuilder.DropIndex(
                name: "IX_OrderHeader_DeliveryEmpId",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "ArrivalDate",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "CancelledEmpId",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "DeliveryEmpId",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "cancelBy",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "deliveryEmpName",
                table: "OrderHeader");
        }
    }
}
