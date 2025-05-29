using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace daemon.Migrations
{
    /// <inheritdoc />
    public partial class Initialdatabasemigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "core");

            migrationBuilder.CreateTable(
                name: "ris_ids",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    discovery_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_seen = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_succeeded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ris_ids", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "stations",
                schema: "core",
                columns: table => new
                {
                    eva_number = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ril100 = table.Column<List<string>>(type: "text[]", nullable: false),
                    querying_enabled = table.Column<bool>(type: "boolean", nullable: true),
                    last_queried = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stations", x => x.eva_number);
                });

            migrationBuilder.CreateTable(
                name: "station_coordinates",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    eva_number = table.Column<int>(type: "integer", nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: true),
                    longitude = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_station_coordinates", x => x.id);
                    table.ForeignKey(
                        name: "FK_station_coordinates_stations_eva_number",
                        column: x => x.eva_number,
                        principalSchema: "core",
                        principalTable: "stations",
                        principalColumn: "eva_number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "station_products",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    eva_number = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    querying_enabled = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_station_products", x => x.id);
                    table.ForeignKey(
                        name: "FK_station_products_stations_eva_number",
                        column: x => x.eva_number,
                        principalSchema: "core",
                        principalTable: "stations",
                        principalColumn: "eva_number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ris_ids_id",
                schema: "core",
                table: "ris_ids",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_station_coordinates_eva_number",
                schema: "core",
                table: "station_coordinates",
                column: "eva_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_station_products_eva_number",
                schema: "core",
                table: "station_products",
                column: "eva_number");

            migrationBuilder.CreateIndex(
                name: "IX_stations_eva_number",
                schema: "core",
                table: "stations",
                column: "eva_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ris_ids",
                schema: "core");

            migrationBuilder.DropTable(
                name: "station_coordinates",
                schema: "core");

            migrationBuilder.DropTable(
                name: "station_products",
                schema: "core");

            migrationBuilder.DropTable(
                name: "stations",
                schema: "core");
        }
    }
}
