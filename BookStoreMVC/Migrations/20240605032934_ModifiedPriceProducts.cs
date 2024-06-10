using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreMVC.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedPriceProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(16,3)",
                precision: 16,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(16,2)",
                oldPrecision: 16,
                oldScale: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(16,2)",
                precision: 16,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(16,3)",
                oldPrecision: 16,
                oldScale: 3);
        }
    }
}
