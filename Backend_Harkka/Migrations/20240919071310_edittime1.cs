using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_Harkka.Migrations
{
    /// <inheritdoc />
    public partial class edittime1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EditTime",
                table: "Messages",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EditTime",
                table: "Messages");
        }
    }
}
