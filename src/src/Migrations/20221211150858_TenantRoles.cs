using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Locker.Migrations
{
    /// <inheritdoc />
    public partial class TenantRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_Roles_AdminRoleID",
                table: "Tenants");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_Roles_UserRoleID",
                table: "Tenants");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_AdminRoleID",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_UserRoleID",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "AdminRoleID",
                table: "Tenants");

            migrationBuilder.RenameColumn(
                name: "UserRoleID",
                table: "Tenants",
                newName: "ApiKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApiKey",
                table: "Tenants",
                newName: "UserRoleID");

            migrationBuilder.AddColumn<string>(
                name: "AdminRoleID",
                table: "Tenants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    ID = table.Column<string>(type: "text", nullable: false),
                    TenantID = table.Column<string>(type: "text", nullable: false),
                    ApiKey = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateLastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    SecurityStamp = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.ID);
                    table.UniqueConstraint("AK_Services_Name", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Services_Tenants_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenants",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_AdminRoleID",
                table: "Tenants",
                column: "AdminRoleID");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_UserRoleID",
                table: "Tenants",
                column: "UserRoleID");

            migrationBuilder.CreateIndex(
                name: "IX_Services_TenantID",
                table: "Services",
                column: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_Roles_AdminRoleID",
                table: "Tenants",
                column: "AdminRoleID",
                principalTable: "Roles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_Roles_UserRoleID",
                table: "Tenants",
                column: "UserRoleID",
                principalTable: "Roles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
