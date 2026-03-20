using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace urbandukanuserservice.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
        table: "Roles",
        columns: ["Id", "Name"],
        values: new object[,]
        {
            { 1, "Admin" },
            { 2, "Seller" },
            { 3, "Buyer" }
        });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
        table: "Roles",
        keyColumn: "Id",
        keyValues: [1, 2, 3]);
        }
    }
}
