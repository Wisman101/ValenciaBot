using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ValenciaBot.Migrations
{
    /// <inheritdoc />
    public partial class updateModels06022023 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OperatingHours");

            migrationBuilder.CreateTable(
                name: "ClinicOperatingHours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    clinicId = table.Column<int>(type: "integer", nullable: false),
                    Days = table.Column<string>(type: "text", nullable: false),
                    Start = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    End = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
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
                    table.PrimaryKey("PK_ClinicOperatingHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClinicOperatingHours_Clinics_clinicId",
                        column: x => x.clinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceOperatingHours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    Days = table.Column<string>(type: "text", nullable: false),
                    Start = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    End = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    ClinicServiceId = table.Column<int>(type: "integer", nullable: true),
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
                    table.PrimaryKey("PK_ServiceOperatingHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceOperatingHours_ClinicServices_ClinicServiceId",
                        column: x => x.ClinicServiceId,
                        principalTable: "ClinicServices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceOperatingHours_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClinicOperatingHours_clinicId",
                table: "ClinicOperatingHours",
                column: "clinicId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOperatingHours_ClinicServiceId",
                table: "ServiceOperatingHours",
                column: "ClinicServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOperatingHours_ServiceId",
                table: "ServiceOperatingHours",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClinicOperatingHours");

            migrationBuilder.DropTable(
                name: "ServiceOperatingHours");

            migrationBuilder.CreateTable(
                name: "OperatingHours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    clinicId = table.Column<int>(type: "integer", nullable: false),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    ClinicServiceId = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Days = table.Column<string>(type: "text", nullable: false),
                    End = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Start = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    entityGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    isActive = table.Column<bool>(type: "boolean", nullable: false),
                    isDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatingHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperatingHours_ClinicServices_ClinicServiceId",
                        column: x => x.ClinicServiceId,
                        principalTable: "ClinicServices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OperatingHours_Clinics_clinicId",
                        column: x => x.clinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OperatingHours_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OperatingHours_clinicId",
                table: "OperatingHours",
                column: "clinicId");

            migrationBuilder.CreateIndex(
                name: "IX_OperatingHours_ClinicServiceId",
                table: "OperatingHours",
                column: "ClinicServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_OperatingHours_ServiceId",
                table: "OperatingHours",
                column: "ServiceId");
        }
    }
}
