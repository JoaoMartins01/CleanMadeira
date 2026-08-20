using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMadeira.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePropertyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoIntermediateCleaning",
                table: "Properties",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "IntermediateCleaningIntervalDays",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoIntermediateCleaning",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "IntermediateCleaningIntervalDays",
                table: "Properties");
        }
    }
}
