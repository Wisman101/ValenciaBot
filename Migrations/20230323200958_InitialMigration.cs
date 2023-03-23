using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ValenciaBot.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UPI = table.Column<string>(type: "text", nullable: true),
                    entityGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    isDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    isActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageSetups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Input = table.Column<string>(type: "text", nullable: false),
                    Response = table.Column<string>(type: "text", nullable: false),
                    isDynamic = table.Column<bool>(type: "boolean", nullable: false),
                    key = table.Column<int>(type: "integer", nullable: false),
                    ParentId = table.Column<int>(type: "integer", nullable: false),
                    entityGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    isDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    isActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageSetups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageSetups_MessageSetups_ParentId",
                        column: x => x.ParentId,
                        principalTable: "MessageSetups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "conversations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    clientId = table.Column<int>(type: "integer", nullable: false),
                    MessageSetupId = table.Column<int>(type: "integer", nullable: false),
                    Input = table.Column<string>(type: "text", nullable: false),
                    Response = table.Column<string>(type: "text", nullable: false),
                    MetaData = table.Column<string>(type: "text", nullable: false),
                    entityGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    isDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    isActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_conversations_Clients_clientId",
                        column: x => x.clientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_conversations_MessageSetups_MessageSetupId",
                        column: x => x.MessageSetupId,
                        principalTable: "MessageSetups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_conversations_clientId",
                table: "conversations",
                column: "clientId");

            migrationBuilder.CreateIndex(
                name: "IX_conversations_MessageSetupId",
                table: "conversations",
                column: "MessageSetupId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageSetups_ParentId",
                table: "MessageSetups",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "conversations");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "MessageSetups");
        }
    }
}
