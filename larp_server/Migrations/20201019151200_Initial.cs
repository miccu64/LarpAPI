using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace larp_server.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Token = table.Column<string>(maxLength: 150, nullable: false),
                    Email = table.Column<string>(maxLength: 80, nullable: true),
                    Name = table.Column<string>(maxLength: 30, nullable: true),
                    Password = table.Column<string>(maxLength: 30, nullable: true),
                    ConnectionID = table.Column<string>(maxLength: 30, nullable: true),
                    IsConnected = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Token);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    Password = table.Column<string>(maxLength: 30, nullable: true),
                    AdminName = table.Column<string>(maxLength: 30, nullable: true),
                    AdminToken = table.Column<string>(nullable: true),
                    LastPlayed = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Rooms_Players_AdminToken",
                        column: x => x.AdminToken,
                        principalTable: "Players",
                        principalColumn: "Token",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Coords",
                columns: table => new
                {
                    RoomId = table.Column<string>(maxLength: 30, nullable: false),
                    PlayerId = table.Column<string>(maxLength: 30, nullable: false),
                    TeamId = table.Column<int>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    Latitude = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coords", x => new { x.PlayerId, x.RoomId });
                    table.ForeignKey(
                        name: "FK_Coords_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Token",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Coords_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coords_RoomId",
                table: "Coords",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_AdminToken",
                table: "Rooms",
                column: "AdminToken");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coords");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
