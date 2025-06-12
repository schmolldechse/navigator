using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace daemon.Migrations
{
    /// <inheritdoc />
    public partial class Addtemporarylock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "querying_enabled",
                schema: "core",
                table: "stations",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_locked",
                schema: "core",
                table: "stations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_locked",
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
                name: "is_locked",
                schema: "core",
                table: "stations");

            migrationBuilder.DropColumn(
                name: "is_locked",
                schema: "core",
                table: "ris_ids");

            migrationBuilder.AlterColumn<bool>(
                name: "querying_enabled",
                schema: "core",
                table: "stations",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }
    }
}
