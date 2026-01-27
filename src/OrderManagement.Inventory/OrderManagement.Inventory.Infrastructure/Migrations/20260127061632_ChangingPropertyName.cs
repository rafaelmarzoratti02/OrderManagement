using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderManagement.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangingPropertyName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "StockItems",
                newName: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "StockItems",
                newName: "Name");
        }
    }
}
