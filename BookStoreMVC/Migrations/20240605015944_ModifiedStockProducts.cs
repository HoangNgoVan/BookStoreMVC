using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreMVC.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedStockProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStock",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStock",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Products");
        }
    }
}
