using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HydrogenBot.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrackedEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    Channel = table.Column<ulong>(nullable: false),
                    MentionString = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TwitchEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    TrackedEventId = table.Column<Guid>(nullable: false),
                    Streamer = table.Column<string>(nullable: false),
                    Online = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwitchEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TwitchEvents_TrackedEvents_TrackedEventId",
                        column: x => x.TrackedEventId,
                        principalTable: "TrackedEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TwitchEvents_TrackedEventId",
                table: "TwitchEvents",
                column: "TrackedEventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TwitchEvents");

            migrationBuilder.DropTable(
                name: "TrackedEvents");
        }
    }
}
