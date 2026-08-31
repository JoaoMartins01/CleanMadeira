using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMadeira.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Properties",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Companies",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "CompanyRole",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Properties_CompanyId",
                table: "Properties",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Companies_CompanyId",
                table: "Properties",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Companies_CompanyId",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_CompanyId",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CompanyRole",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
