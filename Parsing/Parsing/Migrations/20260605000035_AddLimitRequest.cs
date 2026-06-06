using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Parsing.Migrations
{
    /// <inheritdoc />
    public partial class AddLimitRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.CreateTable(
                name: "LimitRequst",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitRequst", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "LimitRequst",
                columns: new[] { "Id", "Time" },
                values: new object[] { 1, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LimitRequst");

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "Ammount", "Balance", "CurentName", "Descriptions", "Time", "TransactionId" },
                values: new object[] { 1, 11113123m, 111L, "Vlad", "Not Found", new DateTimeOffset(new DateTime(2026, 6, 3, 18, 5, 19, 124, DateTimeKind.Unspecified).AddTicks(2338), new TimeSpan(0, 0, 0, 0, 0)), "dsf" });
        }
    }
}
