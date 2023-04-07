using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValenciaBot.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConversationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "log",
                table: "conversations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "sent",
                table: "conversations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "log",
                table: "conversations");

            migrationBuilder.DropColumn(
                name: "sent",
                table: "conversations");
        }
    }
}
