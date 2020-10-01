using Microsoft.EntityFrameworkCore.Migrations;

namespace HydrogenBot.Migrations
{
    public partial class StreamerToStreamerId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE i.* FROM SubscriptionInfo i INNER JOIN TwitchSubscription s ON i.Id = s.SubscriptionInfoId;");

            migrationBuilder.DropColumn(
                name: "Streamer",
                table: "TwitchSubscription");

            migrationBuilder.AddColumn<uint>(
                name: "StreamerId",
                table: "TwitchSubscription",
                nullable: false,
                defaultValue: 0u);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreamerId",
                table: "TwitchSubscription");

            migrationBuilder.AddColumn<string>(
                name: "Streamer",
                table: "TwitchSubscription",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                defaultValue: "");
        }
    }
}
