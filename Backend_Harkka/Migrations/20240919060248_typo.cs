using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_Harkka.Migrations
{
    /// <inheritdoc />
    public partial class typo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_ResipientId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "ResipientId",
                table: "Messages",
                newName: "RecipientId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ResipientId",
                table: "Messages",
                newName: "IX_Messages_RecipientId");

            migrationBuilder.AlterColumn<string>(
                name: "Body",
                table: "Messages",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 5000);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_RecipientId",
                table: "Messages",
                column: "RecipientId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_RecipientId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "RecipientId",
                table: "Messages",
                newName: "ResipientId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_RecipientId",
                table: "Messages",
                newName: "IX_Messages_ResipientId");

            migrationBuilder.AlterColumn<string>(
                name: "Body",
                table: "Messages",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_ResipientId",
                table: "Messages",
                column: "ResipientId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
