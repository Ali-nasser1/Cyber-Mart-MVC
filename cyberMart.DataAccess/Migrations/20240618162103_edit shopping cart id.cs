using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CyberMart.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class editshoppingcartid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "ShoppingCarts",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ShoppingCarts",
                newName: "id");
        }
    }
}
