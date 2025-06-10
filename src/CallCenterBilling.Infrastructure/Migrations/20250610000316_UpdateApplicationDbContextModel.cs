using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CallCenterBilling.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateApplicationDbContextModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 11, 0, 3, 15, 217, DateTimeKind.Utc).AddTicks(7466));

            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 16, 0, 3, 15, 217, DateTimeKind.Utc).AddTicks(7861));

            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 21, 0, 3, 15, 217, DateTimeKind.Utc).AddTicks(7868));

            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 26, 0, 3, 15, 217, DateTimeKind.Utc).AddTicks(7870));

            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 31, 0, 3, 15, 217, DateTimeKind.Utc).AddTicks(7871));

            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 5, 0, 3, 15, 217, DateTimeKind.Utc).AddTicks(7873));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 10, 23, 55, 9, 872, DateTimeKind.Utc).AddTicks(9010));

            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 15, 23, 55, 9, 872, DateTimeKind.Utc).AddTicks(9395));

            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 20, 23, 55, 9, 872, DateTimeKind.Utc).AddTicks(9402));

            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 25, 23, 55, 9, 872, DateTimeKind.Utc).AddTicks(9405));

            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 30, 23, 55, 9, 872, DateTimeKind.Utc).AddTicks(9407));

            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 4, 23, 55, 9, 872, DateTimeKind.Utc).AddTicks(9409));
        }
    }
}
