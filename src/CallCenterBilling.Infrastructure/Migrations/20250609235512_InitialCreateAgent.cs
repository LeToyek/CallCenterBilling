using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CallCenterBilling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateAgent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Agents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalCalls = table.Column<int>(type: "int", nullable: false),
                    TotalRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastActiveAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agents", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Agents",
                columns: new[] { "Id", "CreatedAt", "Email", "LastActiveAt", "Name", "PhoneNumber", "Rating", "Status", "TotalCalls", "TotalRevenue" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 10, 23, 55, 9, 872, DateTimeKind.Utc).AddTicks(9010), "sarah.johnson@company.com", null, "Sarah Johnson", null, 5.0, 1, 89, 2150m },
                    { 2, new DateTime(2025, 5, 15, 23, 55, 9, 872, DateTimeKind.Utc).AddTicks(9395), "mike.chen@company.com", null, "Mike Chen", null, 5.0, 1, 76, 1980m },
                    { 3, new DateTime(2025, 5, 20, 23, 55, 9, 872, DateTimeKind.Utc).AddTicks(9402), "lisa.rodriguez@company.com", null, "Lisa Rodriguez", null, 4.0, 2, 82, 1840m },
                    { 4, new DateTime(2025, 5, 25, 23, 55, 9, 872, DateTimeKind.Utc).AddTicks(9405), "david.smith@company.com", null, "David Smith", null, 4.0, 1, 71, 1720m },
                    { 5, new DateTime(2025, 5, 30, 23, 55, 9, 872, DateTimeKind.Utc).AddTicks(9407), "emma.wilson@company.com", null, "Emma Wilson", null, 4.0, 0, 68, 1650m },
                    { 6, new DateTime(2025, 6, 4, 23, 55, 9, 872, DateTimeKind.Utc).AddTicks(9409), "alex.brown@company.com", null, "Alex Brown", null, 4.0, 1, 65, 1580m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agents_Email",
                table: "Agents",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agents");
        }
    }
}
