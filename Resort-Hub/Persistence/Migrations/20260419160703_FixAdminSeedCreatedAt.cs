using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Resort_Hub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixAdminSeedCreatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6dc6528a-b280-4770-9eae-82671ee81ef7",
                column: "CreatedAt",
                value: new DateTime(2026, 4, 17, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6dc6528a-b280-4770-9eae-82671ee81ef7",
                column: "CreatedAt",
                value: new DateTime(2026, 4, 19, 17, 43, 31, 379, DateTimeKind.Local).AddTicks(8983));
        }
    }
}
