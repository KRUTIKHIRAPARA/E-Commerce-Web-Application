using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce_Web_Application.Migrations
{
    /// <inheritdoc />
    public partial class AddressFiledRemove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
