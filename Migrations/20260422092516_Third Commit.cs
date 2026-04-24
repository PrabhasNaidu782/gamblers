using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamblersGrocery.Migrations
{
    /// <inheritdoc />
    public partial class ThirdCommit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_barcode",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Products_barcode",
                table: "Products",
                column: "barcode",
                unique: true);
        }
    }
}
