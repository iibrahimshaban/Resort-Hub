using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Resort_Hub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationColumnToVilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Villas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Villas");
        }
    }
}
