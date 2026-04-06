using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Navigator.Data.Enums;

#nullable disable

namespace Navigator.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "statistics");

            migrationBuilder.EnsureSchema(
                name: "core");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:core.journey_type", "EXTRA,REGULAR,RELIEF,REPLACEMENT")
                .Annotation("Npgsql:Enum:core.message_type", "ATTRIBUTE,DISRUPTION,NOTE,RIS_CAUSE,RIS_QUALITY_DEVIATION")
                .Annotation("Npgsql:Enum:core.schedule_type", "ARRIVAL,DEPARTURE")
                .Annotation("Npgsql:Enum:core.time_type", "PREVIEW,REAL,SCHEDULE")
                .Annotation("Npgsql:Enum:core.transport_type", "BIKE,BUS,CAR,CITY_TRAIN,FERRY,FLIGHT,HIGH_SPEED_TRAIN,INTERCITY_TRAIN,INTER_REGIONAL_TRAIN,REGIONAL_TRAIN,SCOOTER,SHUTTLE,SUBWAY,TAXI,TRAM,UNKNOWN,WALK")
                .Annotation("Npgsql:PostgresExtension:cube", ",,")
                .Annotation("Npgsql:PostgresExtension:earthdistance", ",,");

            migrationBuilder.CreateTable(
                name: "database_size_snapshots",
                schema: "statistics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    measured_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    size_in_bytes = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_database_size_snapshots", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "journey_snapshots",
                schema: "statistics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    measured_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    total = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journey_snapshots", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ris_ids",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(73)", maxLength: 73, nullable: false),
                    transport_type = table.Column<TransportType>(type: "core.transport_type", nullable: false),
                    replacement_transport_type = table.Column<TransportType>(type: "core.transport_type", nullable: true),
                    discovered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_seen = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_inserted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ris_ids", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "risid_snapshots",
                schema: "statistics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    measured_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    active = table.Column<int>(type: "integer", nullable: false),
                    inactive = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_risid_snapshots", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "stations",
                schema: "core",
                columns: table => new
                {
                    eva_number = table.Column<int>(type: "integer", nullable: false),
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
                    id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    id = table.Column<Guid>(type: "uuid", nullable: false),
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
                name: "IX_database_size_snapshots_measured_at",
                schema: "statistics",
                table: "database_size_snapshots",
                column: "measured_at");

            migrationBuilder.CreateIndex(
                name: "IX_journey_snapshots_measured_at",
                schema: "statistics",
                table: "journey_snapshots",
                column: "measured_at");

            migrationBuilder.CreateIndex(
                name: "IX_ris_ids_active_discovered_at_last_inserted",
                schema: "core",
                table: "ris_ids",
                columns: new[] { "active", "discovered_at", "last_inserted" });

            migrationBuilder.CreateIndex(
                name: "IX_ris_ids_active_last_seen",
                schema: "core",
                table: "ris_ids",
                columns: new[] { "active", "last_seen" });

            migrationBuilder.CreateIndex(
                name: "IX_risid_snapshots_measured_at",
                schema: "statistics",
                table: "risid_snapshots",
                column: "measured_at");

            migrationBuilder.CreateIndex(
                name: "IX_station_ril100_eva_number_ril100",
                schema: "core",
                table: "station_ril100",
                columns: new[] { "eva_number", "ril100" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_station_transports_eva_number_transport_type",
                schema: "core",
                table: "station_transports",
                columns: new[] { "eva_number", "transport_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stations_querying_enabled_last_queried",
                schema: "core",
                table: "stations",
                columns: new[] { "querying_enabled", "last_queried" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "database_size_snapshots",
                schema: "statistics");

            migrationBuilder.DropTable(
                name: "journey_snapshots",
                schema: "statistics");

            migrationBuilder.DropTable(
                name: "ris_ids",
                schema: "core");

            migrationBuilder.DropTable(
                name: "risid_snapshots",
                schema: "statistics");

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
