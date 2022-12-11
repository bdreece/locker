using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Locker.Migrations
{
    /// <inheritdoc />
    public partial class RoleTenants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenantID",
                table: "Roles",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_TenantID",
                table: "Roles",
                column: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Tenants_TenantID",
                table: "Roles",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Tenants_TenantID",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_TenantID",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "TenantID",
                table: "Roles");
        }
    }
}
