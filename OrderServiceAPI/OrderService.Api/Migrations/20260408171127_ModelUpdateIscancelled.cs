using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.Api.Migrations
{
    /// <inheritdoc />
    public partial class ModelUpdateIscancelled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "Orders");
        }
    }
}
