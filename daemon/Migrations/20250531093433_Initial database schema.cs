using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace daemon.Migrations
{
    /// <inheritdoc />
    public partial class Initialdatabaseschema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "core");

            migrationBuilder.CreateTable(
                name: "journey_stop_times",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    planned_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    actual_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    delay = table.Column<int>(type: "integer", nullable: false),
                    planned_platform = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    actual_platform = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journey_stop_times", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "journeys",
                schema: "core",
                columns: table => new
                {
                    journey_id = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    product_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    product_name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    journey_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    journey_name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    operator_code = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    operator_name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journeys", x => x.journey_id);
                    table.CheckConstraint("CK_Trip_JourneyId_Format", "journey_id ~ '^\\d{8}-[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}$'");
                });

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
                    latitude = table.Column<double>(type: "double precision", nullable: true),
                    longitude = table.Column<double>(type: "double precision", nullable: true),
                    querying_enabled = table.Column<bool>(type: "boolean", nullable: true),
                    last_queried = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stations", x => x.eva_number);
                });

            migrationBuilder.CreateTable(
                name: "journey_messages",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    journey_id = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    code = table.Column<int>(type: "integer", nullable: false),
                    message = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    summary = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journey_messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_journey_messages_journeys_journey_id",
                        column: x => x.journey_id,
                        principalSchema: "core",
                        principalTable: "journeys",
                        principalColumn: "journey_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "journey_via-stops",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    journey_id = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    eva_number = table.Column<int>(type: "integer", nullable: false),
                    cancelled = table.Column<bool>(type: "boolean", nullable: false),
                    arrival = table.Column<int>(type: "integer", nullable: true),
                    departure = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journey_via-stops", x => x.id);
                    table.ForeignKey(
                        name: "FK_journey_via-stops_journey_stop_times_arrival",
                        column: x => x.arrival,
                        principalSchema: "core",
                        principalTable: "journey_stop_times",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_journey_via-stops_journey_stop_times_departure",
                        column: x => x.departure,
                        principalSchema: "core",
                        principalTable: "journey_stop_times",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_journey_via-stops_journeys_journey_id",
                        column: x => x.journey_id,
                        principalSchema: "core",
                        principalTable: "journeys",
                        principalColumn: "journey_id",
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
                    name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    querying_enabled = table.Column<bool>(type: "boolean", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "station_ril100",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    eva_number = table.Column<int>(type: "integer", nullable: false),
                    ril100 = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
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
                name: "journey_stop_messages",
                schema: "core",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    stop_id = table.Column<int>(type: "integer", nullable: false),
                    code = table.Column<int>(type: "integer", nullable: false),
                    message = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    summary = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_journey_stop_messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_journey_stop_messages_journey_via-stops_stop_id",
                        column: x => x.stop_id,
                        principalSchema: "core",
                        principalTable: "journey_via-stops",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_journey_messages_journey_id",
                schema: "core",
                table: "journey_messages",
                column: "journey_id");

            migrationBuilder.CreateIndex(
                name: "IX_journey_stop_messages_stop_id",
                schema: "core",
                table: "journey_stop_messages",
                column: "stop_id");

            migrationBuilder.CreateIndex(
                name: "IX_journey_via-stops_arrival",
                schema: "core",
                table: "journey_via-stops",
                column: "arrival");

            migrationBuilder.CreateIndex(
                name: "IX_journey_via-stops_departure",
                schema: "core",
                table: "journey_via-stops",
                column: "departure");

            migrationBuilder.CreateIndex(
                name: "IX_journey_via-stops_journey_id_eva_number",
                schema: "core",
                table: "journey_via-stops",
                columns: new[] { "journey_id", "eva_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_journeys_journey_id",
                schema: "core",
                table: "journeys",
                column: "journey_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ris_ids_id",
                schema: "core",
                table: "ris_ids",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_station_products_eva_number",
                schema: "core",
                table: "station_products",
                column: "eva_number");

            migrationBuilder.CreateIndex(
                name: "IX_station_ril100_eva_number",
                schema: "core",
                table: "station_ril100",
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
                name: "journey_messages",
                schema: "core");

            migrationBuilder.DropTable(
                name: "journey_stop_messages",
                schema: "core");

            migrationBuilder.DropTable(
                name: "ris_ids",
                schema: "core");

            migrationBuilder.DropTable(
                name: "station_products",
                schema: "core");

            migrationBuilder.DropTable(
                name: "station_ril100",
                schema: "core");

            migrationBuilder.DropTable(
                name: "journey_via-stops",
                schema: "core");

            migrationBuilder.DropTable(
                name: "stations",
                schema: "core");

            migrationBuilder.DropTable(
                name: "journey_stop_times",
                schema: "core");

            migrationBuilder.DropTable(
                name: "journeys",
                schema: "core");
        }
    }
}
