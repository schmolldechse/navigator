using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace daemon.Migrations
{
    /// <inheritdoc />
    public partial class Addstationweights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "weight",
                schema: "core",
                table: "stations",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "weight",
                schema: "core",
                table: "stations");
        }
    }
}
