using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parsing.Migrations
{
    /// <inheritdoc />
    public partial class First : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTimeOffset(new DateTime(2026, 6, 3, 18, 5, 19, 124, DateTimeKind.Unspecified).AddTicks(2338), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1,
                column: "Time",
                value: new DateTimeOffset(new DateTime(2026, 6, 3, 18, 4, 35, 421, DateTimeKind.Unspecified).AddTicks(8627), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
