using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMadeira.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCompanyInvitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CompanyInvitations_CompanyId",
                table: "CompanyInvitations",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyInvitations_Companies_CompanyId",
                table: "CompanyInvitations",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyInvitations_Companies_CompanyId",
                table: "CompanyInvitations");

            migrationBuilder.DropIndex(
                name: "IX_CompanyInvitations_CompanyId",
                table: "CompanyInvitations");
        }
    }
}
