using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace daemon.Migrations
{
    /// <inheritdoc />
    public partial class SwitchedCodeType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "code",
                schema: "core",
                table: "journey_stop_messages",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                schema: "core",
                table: "journey_messages",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "code",
                schema: "core",
                table: "journey_stop_messages",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<int>(
                name: "code",
                schema: "core",
                table: "journey_messages",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);
        }
    }
}
