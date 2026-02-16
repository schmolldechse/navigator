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
                .Annotation("Npgsql:Enum:core.journey_type", "EXTRA,REGULAR,RELIEF,REPLACEMENT")
                .Annotation("Npgsql:Enum:core.message_reference_type", "ATTACHMENT,IMAGE,LINK")
                .Annotation("Npgsql:Enum:core.message_type", "ATTRIBUTE,DISRUPTION,NOTE,RIS_CAUSE,RIS_QUALITY_DEVIATION")
                .Annotation("Npgsql:Enum:core.schedule_type", "ARRIVAL,DEPARTURE")
                .Annotation("Npgsql:Enum:core.time_type", "PREVIEW,REAL,SCHEDULE")
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
                    size_in_bytes = table.Column<long>(type: "bigint", nullable: false)
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
                name: "journey_snapshot",
                schema: "statistics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    measured_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    total = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journey_snapshot", x => x.id);
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
                name: "risid_snapshot",
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
                    table.PrimaryKey("PK_risid_snapshot", x => x.id);
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
                name: "journeys",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(82)", maxLength: 82, nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    inserted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    administration_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cancelled = table.Column<bool>(type: "boolean", nullable: false),
                    journey_type = table.Column<JourneyType>(type: "core.journey_type", nullable: false)
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
                name: "journey_messages",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    journey_id = table.Column<string>(type: "character varying(82)", maxLength: 82, nullable: false),
                    message_type = table.Column<MessageType>(type: "core.message_type", nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    text = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    text_short = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    disruption_cause = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    disruption_effect = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    note_category = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journey_messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_journey_messages_journeys_journey_id",
                        column: x => x.journey_id,
                        principalSchema: "core",
                        principalTable: "journeys",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "journey_stop_places",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    journey_id = table.Column<string>(type: "character varying(82)", maxLength: 82, nullable: false),
                    schedule_type = table.Column<ScheduleType>(type: "core.schedule_type", nullable: false),
                    station_eva_number = table.Column<int>(type: "integer", nullable: false),
                    cancelled = table.Column<bool>(type: "boolean", nullable: false),
                    additional = table.Column<bool>(type: "boolean", nullable: false),
                    demand = table.Column<bool>(type: "boolean", nullable: false),
                    no_passenger_change = table.Column<bool>(type: "boolean", nullable: false),
                    planned_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actual_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    time_type = table.Column<TimeType>(type: "core.time_type", nullable: false),
                    delay = table.Column<int>(type: "integer", nullable: false, computedColumnSql: "EXTRACT(EPOCH FROM (actual_time - planned_time))::integer", stored: true),
                    planned_platform = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    actual_platform = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journey_stop_places", x => x.id);
                    table.ForeignKey(
                        name: "FK_journey_stop_places_journeys_journey_id",
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
                    journey_id = table.Column<string>(type: "character varying(82)", maxLength: 82, nullable: false),
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
                name: "journey_message_references",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    message_reference_type = table.Column<MessageReferenceType>(type: "core.message_reference_type", nullable: false),
                    url = table.Column<string>(type: "text", nullable: true),
                    label = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journey_message_references", x => x.id);
                    table.ForeignKey(
                        name: "FK_journey_message_references_journey_messages_message_id",
                        column: x => x.message_id,
                        principalSchema: "core",
                        principalTable: "journey_messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "journey_stop_place_messages",
                schema: "core",
                columns: table => new
                {
                    journey_stop_place_id = table.Column<Guid>(type: "uuid", nullable: false),
                    journey_message_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journey_stop_place_messages", x => new { x.journey_stop_place_id, x.journey_message_id });
                    table.ForeignKey(
                        name: "FK_journey_stop_place_messages_journey_messages_journey_messag~",
                        column: x => x.journey_message_id,
                        principalSchema: "core",
                        principalTable: "journey_messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_journey_stop_place_messages_journey_stop_places_journey_sto~",
                        column: x => x.journey_stop_place_id,
                        principalSchema: "core",
                        principalTable: "journey_stop_places",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_database_size_measured_at",
                schema: "statistics",
                table: "database_size",
                column: "measured_at");

            migrationBuilder.CreateIndex(
                name: "IX_journey_administrations_administration_id_operator_code_ope~",
                schema: "core",
                table: "journey_administrations",
                columns: new[] { "administration_id", "operator_code", "operator_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_journey_message_references_message_id",
                schema: "core",
                table: "journey_message_references",
                column: "message_id");

            migrationBuilder.CreateIndex(
                name: "IX_journey_messages_journey_id",
                schema: "core",
                table: "journey_messages",
                column: "journey_id");

            migrationBuilder.CreateIndex(
                name: "IX_journey_messages_message_type",
                schema: "core",
                table: "journey_messages",
                column: "message_type");

            migrationBuilder.CreateIndex(
                name: "IX_journey_snapshot_measured_at",
                schema: "statistics",
                table: "journey_snapshot",
                column: "measured_at");

            migrationBuilder.CreateIndex(
                name: "IX_journey_stop_place_messages_journey_message_id",
                schema: "core",
                table: "journey_stop_place_messages",
                column: "journey_message_id");

            migrationBuilder.CreateIndex(
                name: "IX_journey_stop_places_actual_time",
                schema: "core",
                table: "journey_stop_places",
                column: "actual_time");

            migrationBuilder.CreateIndex(
                name: "IX_journey_stop_places_journey_id",
                schema: "core",
                table: "journey_stop_places",
                column: "journey_id");

            migrationBuilder.CreateIndex(
                name: "IX_journey_stop_places_planned_time",
                schema: "core",
                table: "journey_stop_places",
                column: "planned_time");

            migrationBuilder.CreateIndex(
                name: "IX_journey_stop_places_station_analytics",
                schema: "core",
                table: "journey_stop_places",
                columns: new[] { "station_eva_number", "planned_time" })
                .Annotation("Npgsql:IndexInclude", new[] { "journey_id", "schedule_type", "delay", "cancelled", "additional", "demand", "no_passenger_change", "planned_platform", "actual_platform" });

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
                name: "IX_journeys_date",
                schema: "core",
                table: "journeys",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "IX_journeys_inserted_at",
                schema: "core",
                table: "journeys",
                column: "inserted_at");

            migrationBuilder.CreateIndex(
                name: "IX_journeys_journey_type",
                schema: "core",
                table: "journeys",
                column: "journey_type");

            migrationBuilder.CreateIndex(
                name: "IX_ris_ids_active_last_seen",
                schema: "core",
                table: "ris_ids",
                columns: new[] { "active", "last_seen" });

            migrationBuilder.CreateIndex(
                name: "IX_ris_ids_discovered_at",
                schema: "core",
                table: "ris_ids",
                column: "discovered_at");

            migrationBuilder.CreateIndex(
                name: "IX_ris_ids_last_inserted",
                schema: "core",
                table: "ris_ids",
                column: "last_inserted");

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
                name: "IX_risid_snapshot_measured_at",
                schema: "statistics",
                table: "risid_snapshot",
                column: "measured_at");

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
                name: "database_size",
                schema: "statistics");

            migrationBuilder.DropTable(
                name: "journey_message_references",
                schema: "core");

            migrationBuilder.DropTable(
                name: "journey_snapshot",
                schema: "statistics");

            migrationBuilder.DropTable(
                name: "journey_stop_place_messages",
                schema: "core");

            migrationBuilder.DropTable(
                name: "journey_transports",
                schema: "core");

            migrationBuilder.DropTable(
                name: "ris_ids",
                schema: "core");

            migrationBuilder.DropTable(
                name: "risid_snapshot",
                schema: "statistics");

            migrationBuilder.DropTable(
                name: "station_ril100",
                schema: "core");

            migrationBuilder.DropTable(
                name: "station_transports",
                schema: "core");

            migrationBuilder.DropTable(
                name: "journey_messages",
                schema: "core");

            migrationBuilder.DropTable(
                name: "journey_stop_places",
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
