using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMadeira.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nif",
                table: "Companies",
                newName: "NIF");

            migrationBuilder.RenameColumn(
                name: "Adress",
                table: "Companies",
                newName: "Address");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NIF",
                table: "Companies",
                newName: "Nif");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Companies",
                newName: "Adress");
        }
    }
}
