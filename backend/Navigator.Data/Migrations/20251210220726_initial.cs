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
                .Annotation("Npgsql:Enum:core.information_type", "DISRUPTION,JOURNEY_ATTRIBUTE,MESSAGE,RIS_CAUSE_REASON,RIS_QUALITY_DEVIATION")
                .Annotation("Npgsql:Enum:core.schedule_type", "ARRIVAL,DEPARTURE")
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
                name: "journey_administrations",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    administration_id = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    operator_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    operator_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journey_administrations", x => x.id);
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
                name: "journeys",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    inserted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    administration_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cancelled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journeys", x => x.id);
                    table.ForeignKey(
                        name: "FK_journeys_journey_administrations_administration_id",
                        column: x => x.administration_id,
                        principalSchema: "core",
                        principalTable: "journey_administrations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "journey_scheduled_stop_places",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    journey_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    schedule_type = table.Column<ScheduleType>(type: "core.schedule_type", nullable: false),
                    station_name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    station_eva_number = table.Column<int>(type: "integer", nullable: false),
                    cancelled = table.Column<bool>(type: "boolean", nullable: false),
                    additional = table.Column<bool>(type: "boolean", nullable: false),
                    demand = table.Column<bool>(type: "boolean", nullable: false),
                    no_passenger_change = table.Column<bool>(type: "boolean", nullable: false),
                    planned_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actual_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    delay = table.Column<int>(type: "integer", nullable: false, computedColumnSql: "EXTRACT(EPOCH FROM (actual_time - planned_time))::integer", stored: true),
                    planned_platform = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    actual_platform = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journey_scheduled_stop_places", x => x.id);
                    table.ForeignKey(
                        name: "FK_journey_scheduled_stop_places_journeys_journey_id",
                        column: x => x.journey_id,
                        principalSchema: "core",
                        principalTable: "journeys",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "journey_transports",
                schema: "core",
                columns: table => new
                {
                    journey_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transport_type = table.Column<TransportType>(type: "core.transport_type", nullable: false),
                    replacement_transport_type = table.Column<TransportType>(type: "core.transport_type", nullable: true),
                    category = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    category_internal = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    journey_description = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    label = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    line = table.Column<string>(type: "text", nullable: true),
                    number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journey_transports", x => x.journey_id);
                    table.ForeignKey(
                        name: "FK_journey_transports_journeys_journey_id",
                        column: x => x.journey_id,
                        principalSchema: "core",
                        principalTable: "journeys",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "journey_stop_place_informations",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    scheduled_stop_place_id = table.Column<Guid>(type: "uuid", nullable: false),
                    information_type = table.Column<InformationType>(type: "core.information_type", nullable: false),
                    key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    text = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    text_short = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    disruption_communication_id = table.Column<Guid>(type: "uuid", nullable: true),
                    disruption_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journey_stop_place_informations", x => x.id);
                    table.ForeignKey(
                        name: "FK_journey_stop_place_informations_journey_scheduled_stop_plac~",
                        column: x => x.scheduled_stop_place_id,
                        principalSchema: "core",
                        principalTable: "journey_scheduled_stop_places",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_journey_administrations_administration_id_operator_code_ope~",
                schema: "core",
                table: "journey_administrations",
                columns: new[] { "administration_id", "operator_code", "operator_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_journey_scheduled_stop_places_date",
                schema: "core",
                table: "journey_scheduled_stop_places",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "IX_journey_scheduled_stop_places_journey_id",
                schema: "core",
                table: "journey_scheduled_stop_places",
                column: "journey_id");

            migrationBuilder.CreateIndex(
                name: "IX_journey_scheduled_stop_places_station_eva_number",
                schema: "core",
                table: "journey_scheduled_stop_places",
                column: "station_eva_number");

            migrationBuilder.CreateIndex(
                name: "IX_journey_scheduled_stop_places_station_eva_number_date",
                schema: "core",
                table: "journey_scheduled_stop_places",
                columns: new[] { "station_eva_number", "date" });

            migrationBuilder.CreateIndex(
                name: "IX_journey_scheduled_stop_places_station_eva_number_journey_id",
                schema: "core",
                table: "journey_scheduled_stop_places",
                columns: new[] { "station_eva_number", "journey_id" });

            migrationBuilder.CreateIndex(
                name: "IX_journey_stop_place_informations_information_type",
                schema: "core",
                table: "journey_stop_place_informations",
                column: "information_type");

            migrationBuilder.CreateIndex(
                name: "IX_journey_stop_place_informations_key",
                schema: "core",
                table: "journey_stop_place_informations",
                column: "key");

            migrationBuilder.CreateIndex(
                name: "IX_journey_stop_place_informations_scheduled_stop_place_id",
                schema: "core",
                table: "journey_stop_place_informations",
                column: "scheduled_stop_place_id");

            migrationBuilder.CreateIndex(
                name: "IX_journey_transports_category",
                schema: "core",
                table: "journey_transports",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_journey_transports_journey_description",
                schema: "core",
                table: "journey_transports",
                column: "journey_description");

            migrationBuilder.CreateIndex(
                name: "IX_journey_transports_number",
                schema: "core",
                table: "journey_transports",
                column: "number");

            migrationBuilder.CreateIndex(
                name: "IX_journey_transports_replacement_transport_type",
                schema: "core",
                table: "journey_transports",
                column: "replacement_transport_type");

            migrationBuilder.CreateIndex(
                name: "IX_journey_transports_transport_type",
                schema: "core",
                table: "journey_transports",
                column: "transport_type");

            migrationBuilder.CreateIndex(
                name: "IX_journeys_administration_id",
                schema: "core",
                table: "journeys",
                column: "administration_id");

            migrationBuilder.CreateIndex(
                name: "IX_journeys_cancelled",
                schema: "core",
                table: "journeys",
                column: "cancelled");

            migrationBuilder.CreateIndex(
                name: "IX_journeys_date",
                schema: "core",
                table: "journeys",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "IX_ris_ids_replacement_transport_type",
                schema: "core",
                table: "ris_ids",
                column: "replacement_transport_type");

            migrationBuilder.CreateIndex(
                name: "IX_ris_ids_transport_type",
                schema: "core",
                table: "ris_ids",
                column: "transport_type");

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
                name: "journey_stop_place_informations",
                schema: "core");

            migrationBuilder.DropTable(
                name: "journey_transports",
                schema: "core");

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
                name: "journey_scheduled_stop_places",
                schema: "core");

            migrationBuilder.DropTable(
                name: "stations",
                schema: "core");

            migrationBuilder.DropTable(
                name: "journeys",
                schema: "core");

            migrationBuilder.DropTable(
                name: "journey_administrations",
                schema: "core");
        }
    }
}
