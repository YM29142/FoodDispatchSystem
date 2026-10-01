using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDispatchSystem.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddTipAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TipAmount",
                table: "Orders",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipAmount",
                table: "Orders");
        }
    }
}
