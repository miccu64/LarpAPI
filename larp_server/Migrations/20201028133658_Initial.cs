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
                    Nickname = table.Column<string>(maxLength: 30, nullable: false),
                    Token = table.Column<string>(maxLength: 150, nullable: true),
                    Email = table.Column<string>(maxLength: 80, nullable: true),
                    Password = table.Column<string>(maxLength: 30, nullable: true),
                    ConnectionID = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Nickname);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    Password = table.Column<string>(maxLength: 30, nullable: true),
                    LastPlayed = table.Column<DateTime>(nullable: false),
                    AdminNickname = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Rooms_Players_AdminNickname",
                        column: x => x.AdminNickname,
                        principalTable: "Players",
                        principalColumn: "Nickname",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Coords",
                columns: table => new
                {
                    RoomName = table.Column<string>(maxLength: 30, nullable: false),
                    PlayerName = table.Column<string>(maxLength: 30, nullable: false),
                    TeamId = table.Column<int>(nullable: false),
                    IsConnected = table.Column<bool>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    PlayerNickname = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coords", x => new { x.PlayerName, x.RoomName });
                    table.ForeignKey(
                        name: "FK_Coords_Players_PlayerNickname",
                        column: x => x.PlayerNickname,
                        principalTable: "Players",
                        principalColumn: "Nickname",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Coords_Rooms_RoomName",
                        column: x => x.RoomName,
                        principalTable: "Rooms",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coords_PlayerNickname",
                table: "Coords",
                column: "PlayerNickname");

            migrationBuilder.CreateIndex(
                name: "IX_Coords_RoomName",
                table: "Coords",
                column: "RoomName");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_AdminNickname",
                table: "Rooms",
                column: "AdminNickname");
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
