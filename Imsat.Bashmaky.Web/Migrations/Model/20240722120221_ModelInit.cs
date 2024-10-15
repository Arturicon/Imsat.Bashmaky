using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Imsat.Bashmaky.Web.Migrations.Model
{
    /// <inheritdoc />
    public partial class ModelInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "model");

            migrationBuilder.CreateSequence(
                name: "BaseEntitySequence",
                schema: "model");

            migrationBuilder.CreateTable(
                name: "Railways",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Railways", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stations",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrainAttachments",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AttachingTimeUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DetachingTimeUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RailwayNumber = table.Column<int>(type: "integer", nullable: false),
                    WagonNumber = table.Column<int>(type: "integer", nullable: false),
                    AttachingFitter = table.Column<string>(type: "text", nullable: false),
                    DetachingFitter = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainAttachments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaseEntity",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('model.\"BaseEntitySequence\"')"),
                    TS = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Imei = table.Column<string>(type: "text", nullable: true),
                    StationId = table.Column<int>(type: "integer", nullable: true),
                    LAT = table.Column<float>(type: "real", nullable: false),
                    LON = table.Column<float>(type: "real", nullable: false),
                    VDD = table.Column<double>(type: "double precision", nullable: false),
                    St = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaseEntity_Stations_StationId",
                        column: x => x.StationId,
                        principalSchema: "model",
                        principalTable: "Stations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BashmakSignals",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('model.\"BaseEntitySequence\"')"),
                    TS = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Imei = table.Column<string>(type: "text", nullable: true),
                    StationId = table.Column<int>(type: "integer", nullable: true),
                    LAT = table.Column<float>(type: "real", nullable: false),
                    LON = table.Column<float>(type: "real", nullable: false),
                    VDD = table.Column<double>(type: "double precision", nullable: false),
                    St = table.Column<int>(type: "integer", nullable: false),
                    Mac = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BashmakSignals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BashmakSignals_Stations_StationId",
                        column: x => x.StationId,
                        principalSchema: "model",
                        principalTable: "Stations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Boxes",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('model.\"BaseEntitySequence\"')"),
                    TS = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Imei = table.Column<string>(type: "text", nullable: true),
                    StationId = table.Column<int>(type: "integer", nullable: true),
                    LAT = table.Column<float>(type: "real", nullable: false),
                    LON = table.Column<float>(type: "real", nullable: false),
                    VDD = table.Column<double>(type: "double precision", nullable: false),
                    St = table.Column<int>(type: "integer", nullable: false),
                    Mac = table.Column<string>(type: "text", nullable: false),
                    Connection = table.Column<int>(type: "integer", nullable: false),
                    DeviceName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Boxes_Stations_StationId",
                        column: x => x.StationId,
                        principalSchema: "model",
                        principalTable: "Stations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BoxSignals",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('model.\"BaseEntitySequence\"')"),
                    TS = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Imei = table.Column<string>(type: "text", nullable: true),
                    StationId = table.Column<int>(type: "integer", nullable: true),
                    LAT = table.Column<float>(type: "real", nullable: false),
                    LON = table.Column<float>(type: "real", nullable: false),
                    VDD = table.Column<double>(type: "double precision", nullable: false),
                    St = table.Column<int>(type: "integer", nullable: false),
                    Mac = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoxSignals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoxSignals_Stations_StationId",
                        column: x => x.StationId,
                        principalSchema: "model",
                        principalTable: "Stations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Terminals",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('model.\"BaseEntitySequence\"')"),
                    TS = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Imei = table.Column<string>(type: "text", nullable: true),
                    StationId = table.Column<int>(type: "integer", nullable: true),
                    LAT = table.Column<float>(type: "real", nullable: false),
                    LON = table.Column<float>(type: "real", nullable: false),
                    VDD = table.Column<double>(type: "double precision", nullable: false),
                    St = table.Column<int>(type: "integer", nullable: false),
                    NET = table.Column<string>(type: "text", nullable: true),
                    RSSI = table.Column<int>(type: "integer", nullable: false),
                    Connection = table.Column<int>(type: "integer", nullable: false),
                    DeviceName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terminals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Terminals_Stations_StationId",
                        column: x => x.StationId,
                        principalSchema: "model",
                        principalTable: "Stations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TerminalSignals",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('model.\"BaseEntitySequence\"')"),
                    TS = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Imei = table.Column<string>(type: "text", nullable: true),
                    StationId = table.Column<int>(type: "integer", nullable: true),
                    LAT = table.Column<float>(type: "real", nullable: false),
                    LON = table.Column<float>(type: "real", nullable: false),
                    VDD = table.Column<double>(type: "double precision", nullable: false),
                    St = table.Column<int>(type: "integer", nullable: false),
                    NET = table.Column<string>(type: "text", nullable: true),
                    RSSI = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminalSignals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TerminalSignals_Stations_StationId",
                        column: x => x.StationId,
                        principalSchema: "model",
                        principalTable: "Stations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Bashmaks",
                schema: "model",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('model.\"BaseEntitySequence\"')"),
                    TS = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Imei = table.Column<string>(type: "text", nullable: true),
                    StationId = table.Column<int>(type: "integer", nullable: true),
                    LAT = table.Column<float>(type: "real", nullable: false),
                    LON = table.Column<float>(type: "real", nullable: false),
                    VDD = table.Column<double>(type: "double precision", nullable: false),
                    St = table.Column<int>(type: "integer", nullable: false),
                    Mac = table.Column<string>(type: "text", nullable: false),
                    BoxId = table.Column<int>(type: "integer", nullable: true),
                    TrainAttachmentId = table.Column<int>(type: "integer", nullable: true),
                    Connection = table.Column<int>(type: "integer", nullable: false),
                    DeviceName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bashmaks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bashmaks_Boxes_BoxId",
                        column: x => x.BoxId,
                        principalSchema: "model",
                        principalTable: "Boxes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bashmaks_Stations_StationId",
                        column: x => x.StationId,
                        principalSchema: "model",
                        principalTable: "Stations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bashmaks_TrainAttachments_TrainAttachmentId",
                        column: x => x.TrainAttachmentId,
                        principalSchema: "model",
                        principalTable: "TrainAttachments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BashmakRailway",
                schema: "model",
                columns: table => new
                {
                    BashmaksId = table.Column<int>(type: "integer", nullable: false),
                    RailwaysId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BashmakRailway", x => new { x.BashmaksId, x.RailwaysId });
                    table.ForeignKey(
                        name: "FK_BashmakRailway_Bashmaks_BashmaksId",
                        column: x => x.BashmaksId,
                        principalSchema: "model",
                        principalTable: "Bashmaks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BashmakRailway_Railways_RailwaysId",
                        column: x => x.RailwaysId,
                        principalSchema: "model",
                        principalTable: "Railways",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaseEntity_StationId",
                schema: "model",
                table: "BaseEntity",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_BashmakRailway_RailwaysId",
                schema: "model",
                table: "BashmakRailway",
                column: "RailwaysId");

            migrationBuilder.CreateIndex(
                name: "IX_Bashmaks_BoxId",
                schema: "model",
                table: "Bashmaks",
                column: "BoxId");

            migrationBuilder.CreateIndex(
                name: "IX_Bashmaks_StationId",
                schema: "model",
                table: "Bashmaks",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_Bashmaks_TrainAttachmentId",
                schema: "model",
                table: "Bashmaks",
                column: "TrainAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_BashmakSignals_StationId",
                schema: "model",
                table: "BashmakSignals",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_Boxes_StationId",
                schema: "model",
                table: "Boxes",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_BoxSignals_StationId",
                schema: "model",
                table: "BoxSignals",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_Terminals_StationId",
                schema: "model",
                table: "Terminals",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_TerminalSignals_StationId",
                schema: "model",
                table: "TerminalSignals",
                column: "StationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaseEntity",
                schema: "model");

            migrationBuilder.DropTable(
                name: "BashmakRailway",
                schema: "model");

            migrationBuilder.DropTable(
                name: "BashmakSignals",
                schema: "model");

            migrationBuilder.DropTable(
                name: "BoxSignals",
                schema: "model");

            migrationBuilder.DropTable(
                name: "Terminals",
                schema: "model");

            migrationBuilder.DropTable(
                name: "TerminalSignals",
                schema: "model");

            migrationBuilder.DropTable(
                name: "Bashmaks",
                schema: "model");

            migrationBuilder.DropTable(
                name: "Railways",
                schema: "model");

            migrationBuilder.DropTable(
                name: "Boxes",
                schema: "model");

            migrationBuilder.DropTable(
                name: "TrainAttachments",
                schema: "model");

            migrationBuilder.DropTable(
                name: "Stations",
                schema: "model");

            migrationBuilder.DropSequence(
                name: "BaseEntitySequence",
                schema: "model");
        }
    }
}
