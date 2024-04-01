using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceWebAppProject.DB.Migrations
{
    /// <inheritdoc />
    public partial class addShoppingCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Category_catId",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "catId",
                table: "Product",
                newName: "CatId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_catId",
                table: "Product",
                newName: "IX_Product_CatId");

            migrationBuilder.RenameColumn(
                name: "village",
                table: "AspNetUsers",
                newName: "Village");

            migrationBuilder.RenameColumn(
                name: "streetName",
                table: "AspNetUsers",
                newName: "StreetName");

            migrationBuilder.RenameColumn(
                name: "postalNumber",
                table: "AspNetUsers",
                newName: "PostalNumber");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "AspNetUsers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "homeNumber",
                table: "AspNetUsers",
                newName: "HomeNumber");

            migrationBuilder.RenameColumn(
                name: "commune",
                table: "AspNetUsers",
                newName: "Commune");

            migrationBuilder.RenameColumn(
                name: "city",
                table: "AspNetUsers",
                newName: "City");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateTable(
                name: "ShoppingCart",
                columns: table => new
                {
                    cartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productId = table.Column<int>(type: "int", nullable: false),
                    qauntity = table.Column<int>(type: "int", nullable: false),
                    appUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCart", x => x.cartId);
                    table.ForeignKey(
                        name: "FK_ShoppingCart_AspNetUsers_appUserId",
                        column: x => x.appUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShoppingCart_Product_productId",
                        column: x => x.productId,
                        principalTable: "Product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_appUserId",
                table: "ShoppingCart",
                column: "appUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_productId",
                table: "ShoppingCart",
                column: "productId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Category_CatId",
                table: "Product",
                column: "CatId",
                principalTable: "Category",
                principalColumn: "CatId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Category_CatId",
                table: "Product");

            migrationBuilder.DropTable(
                name: "ShoppingCart");

            migrationBuilder.RenameColumn(
                name: "CatId",
                table: "Product",
                newName: "catId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_CatId",
                table: "Product",
                newName: "IX_Product_catId");

            migrationBuilder.RenameColumn(
                name: "Village",
                table: "AspNetUsers",
                newName: "village");

            migrationBuilder.RenameColumn(
                name: "StreetName",
                table: "AspNetUsers",
                newName: "streetName");

            migrationBuilder.RenameColumn(
                name: "PostalNumber",
                table: "AspNetUsers",
                newName: "postalNumber");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "AspNetUsers",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "HomeNumber",
                table: "AspNetUsers",
                newName: "homeNumber");

            migrationBuilder.RenameColumn(
                name: "Commune",
                table: "AspNetUsers",
                newName: "commune");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "AspNetUsers",
                newName: "city");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Category_catId",
                table: "Product",
                column: "catId",
                principalTable: "Category",
                principalColumn: "CatId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
