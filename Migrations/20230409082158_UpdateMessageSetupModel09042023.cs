using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValenciaBot.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMessageSetupModel09042023 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "Users",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Users",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "entityGuid",
                table: "Users",
                newName: "EntityGuid");

            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "Services",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Services",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "entityGuid",
                table: "Services",
                newName: "EntityGuid");

            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "ServiceOperatingHours",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "ServiceOperatingHours",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "entityGuid",
                table: "ServiceOperatingHours",
                newName: "EntityGuid");

            migrationBuilder.RenameColumn(
                name: "key",
                table: "MessageSetups",
                newName: "Key");

            migrationBuilder.RenameColumn(
                name: "isDynamic",
                table: "MessageSetups",
                newName: "IsDynamic");

            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "MessageSetups",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "MessageSetups",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "entityGuid",
                table: "MessageSetups",
                newName: "EntityGuid");

            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "conversations",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "conversations",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "entityGuid",
                table: "conversations",
                newName: "EntityGuid");

            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "ClinicServices",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "ClinicServices",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "entityGuid",
                table: "ClinicServices",
                newName: "EntityGuid");

            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "Clinics",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Clinics",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "entityGuid",
                table: "Clinics",
                newName: "EntityGuid");

            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "ClinicOperatingHours",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "ClinicOperatingHours",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "entityGuid",
                table: "ClinicOperatingHours",
                newName: "EntityGuid");

            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "Clients",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Clients",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "entityGuid",
                table: "Clients",
                newName: "EntityGuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Users",
                newName: "isDeleted");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Users",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "EntityGuid",
                table: "Users",
                newName: "entityGuid");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Services",
                newName: "isDeleted");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Services",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "EntityGuid",
                table: "Services",
                newName: "entityGuid");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "ServiceOperatingHours",
                newName: "isDeleted");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "ServiceOperatingHours",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "EntityGuid",
                table: "ServiceOperatingHours",
                newName: "entityGuid");

            migrationBuilder.RenameColumn(
                name: "Key",
                table: "MessageSetups",
                newName: "key");

            migrationBuilder.RenameColumn(
                name: "IsDynamic",
                table: "MessageSetups",
                newName: "isDynamic");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "MessageSetups",
                newName: "isDeleted");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "MessageSetups",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "EntityGuid",
                table: "MessageSetups",
                newName: "entityGuid");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "conversations",
                newName: "isDeleted");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "conversations",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "EntityGuid",
                table: "conversations",
                newName: "entityGuid");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "ClinicServices",
                newName: "isDeleted");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "ClinicServices",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "EntityGuid",
                table: "ClinicServices",
                newName: "entityGuid");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Clinics",
                newName: "isDeleted");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Clinics",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "EntityGuid",
                table: "Clinics",
                newName: "entityGuid");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "ClinicOperatingHours",
                newName: "isDeleted");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "ClinicOperatingHours",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "EntityGuid",
                table: "ClinicOperatingHours",
                newName: "entityGuid");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Clients",
                newName: "isDeleted");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Clients",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "EntityGuid",
                table: "Clients",
                newName: "entityGuid");
        }
    }
}
