using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Navigator.Data.Enums;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Navigator.Data.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "statistics");

            migrationBuilder.EnsureSchema(
                name: "core");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:core.transport_type", "BIKE,BUS,CAR,CITY_TRAIN,FERRY,FLIGHT,HIGH_SPEED_TRAIN,INTERCITY_TRAIN,INTER_REGIONAL_TRAIN,REGIONAL_TRAIN,SCOOTER,SHUTTLE,SUBWAY,TAXI,TRAM,UNKNOWN,WALK")
                .Annotation("Npgsql:PostgresExtension:cube", ",,")
                .Annotation("Npgsql:PostgresExtension:earthdistance", ",,");

            migrationBuilder.CreateTable(
                name: "database_size",
                schema: "statistics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    measured_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    size_bytes = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_database_size", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ris_ids",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transport_type = table.Column<TransportType>(type: "core.transport_type", nullable: false),
                    replacement_transport_type = table.Column<TransportType>(type: "core.transport_type", nullable: true),
                    discovered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_insertion_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    active = table.Column<bool>(type: "boolean", nullable: false)
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
                    name = table.Column<string>(type: "text", nullable: false),
                    weight = table.Column<double>(type: "double precision", nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    querying_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    last_queried = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stations", x => x.eva_number);
                });

            migrationBuilder.CreateTable(
                name: "station_ril100",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    eva_number = table.Column<int>(type: "integer", nullable: false),
                    ril100 = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_station_ril100", x => x.id);
                    table.ForeignKey(
                        name: "FK_station_ril100_stations_eva_number",
                        column: x => x.eva_number,
                        principalSchema: "core",
                        principalTable: "stations",
                        principalColumn: "eva_number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "station_transports",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    eva_number = table.Column<int>(type: "integer", nullable: false),
                    transport_type = table.Column<TransportType>(type: "core.transport_type", nullable: false),
                    enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_station_transports", x => x.id);
                    table.ForeignKey(
                        name: "FK_station_transports_stations_eva_number",
                        column: x => x.eva_number,
                        principalSchema: "core",
                        principalTable: "stations",
                        principalColumn: "eva_number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_station_ril100_eva_number",
                schema: "core",
                table: "station_ril100",
                column: "eva_number");

            migrationBuilder.CreateIndex(
                name: "IX_station_transports_eva_number_transport_type",
                schema: "core",
                table: "station_transports",
                columns: new[] { "eva_number", "transport_type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "database_size",
                schema: "statistics");

            migrationBuilder.DropTable(
                name: "ris_ids",
                schema: "core");

            migrationBuilder.DropTable(
                name: "station_ril100",
                schema: "core");

            migrationBuilder.DropTable(
                name: "station_transports",
                schema: "core");

            migrationBuilder.DropTable(
                name: "stations",
                schema: "core");
        }
    }
}
