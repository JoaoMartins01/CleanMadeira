using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanMadeira.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRejectionReasonToMaintenance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Maintenances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Maintenances");
        }
    }
}
