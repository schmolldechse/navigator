using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace stations.Migrations
{
    /// <inheritdoc />
    public partial class EnsureEvaNumberisunique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<string>>(
                name: "Ril100",
                schema: "data",
                table: "stations",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_stations_EvaNumber",
                schema: "data",
                table: "stations",
                column: "EvaNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_stations_EvaNumber",
                schema: "data",
                table: "stations");

            migrationBuilder.AlterColumn<List<string>>(
                name: "Ril100",
                schema: "data",
                table: "stations",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "text[]");
        }
    }
}
