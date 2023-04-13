using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ValenciaBot.Migrations
{
    /// <inheritdoc />
    public partial class SeparateMessagesFromConversation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Input",
                table: "conversations");

            migrationBuilder.DropColumn(
                name: "MetaData",
                table: "conversations");

            migrationBuilder.DropColumn(
                name: "Response",
                table: "conversations");

            migrationBuilder.DropColumn(
                name: "log",
                table: "conversations");

            migrationBuilder.DropColumn(
                name: "sent",
                table: "conversations");

            migrationBuilder.RenameColumn(
                name: "serviceId",
                table: "conversations",
                newName: "TransitData");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "conversations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MessageSetupId = table.Column<int>(type: "integer", nullable: false),
                    Input = table.Column<string>(type: "text", nullable: false),
                    Response = table.Column<string>(type: "text", nullable: false),
                    MetaData = table.Column<string>(type: "text", nullable: false),
                    sent = table.Column<bool>(type: "boolean", nullable: false),
                    log = table.Column<string>(type: "text", nullable: true),
                    ConversationId = table.Column<int>(type: "integer", nullable: true),
                    EntityGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_MessageSetups_MessageSetupId",
                        column: x => x.MessageSetupId,
                        principalTable: "MessageSetups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "conversations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationId",
                table: "Messages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MessageSetupId",
                table: "Messages",
                column: "MessageSetupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropColumn(
                name: "status",
                table: "conversations");

            migrationBuilder.RenameColumn(
                name: "TransitData",
                table: "conversations",
                newName: "serviceId");

            migrationBuilder.AddColumn<string>(
                name: "Input",
                table: "conversations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MetaData",
                table: "conversations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Response",
                table: "conversations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "log",
                table: "conversations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "sent",
                table: "conversations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
