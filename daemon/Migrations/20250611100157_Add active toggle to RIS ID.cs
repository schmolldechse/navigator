using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace daemon.Migrations
{
    /// <inheritdoc />
    public partial class AddactivetoggletoRISID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "active",
                schema: "core",
                table: "ris_ids",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "active",
                schema: "core",
                table: "ris_ids");
        }
    }
}
