using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace stations.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "data");

            migrationBuilder.CreateTable(
                name: "stations",
                schema: "data",
                columns: table => new
                {
                    EvaNumber = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Ril100 = table.Column<List<string>>(type: "text[]", nullable: true),
                    Products = table.Column<List<string>>(type: "text[]", nullable: false),
                    QueryingEnabled = table.Column<bool>(type: "boolean", nullable: true),
                    LastQueried = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stations", x => x.EvaNumber);
                });

            migrationBuilder.CreateTable(
                name: "station_coordinates",
                schema: "data",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EvaNumber = table.Column<int>(type: "integer", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_station_coordinates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_station_coordinates_stations_EvaNumber",
                        column: x => x.EvaNumber,
                        principalSchema: "data",
                        principalTable: "stations",
                        principalColumn: "EvaNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_station_coordinates_EvaNumber",
                schema: "data",
                table: "station_coordinates",
                column: "EvaNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "station_coordinates",
                schema: "data");

            migrationBuilder.DropTable(
                name: "stations",
                schema: "data");
        }
    }
}
