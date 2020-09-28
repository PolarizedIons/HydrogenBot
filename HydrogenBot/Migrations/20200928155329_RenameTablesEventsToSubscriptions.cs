using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HydrogenBot.Migrations
{
    public partial class RenameTablesEventsToSubscriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_TwitchEvents_TrackedEvents_TrackedEventId", "TwitchEvents");
            migrationBuilder.DropIndex("IX_TwitchEvents_TrackedEventId", "TwitchEvents");
            migrationBuilder.DropPrimaryKey("PK_TrackedEvents", "TrackedEvents");
            migrationBuilder.DropPrimaryKey("PK_TwitchEvents", "TwitchEvents");
            
            migrationBuilder.RenameColumn("TrackedEventId", "TwitchEvents", "SubscriptionInfoId");
            
            migrationBuilder.RenameTable("TrackedEvents", newName: "SubscriptionInfo");
            migrationBuilder.RenameTable("TwitchEvents", newName: "TwitchSubscription");
            
            migrationBuilder.AddPrimaryKey("PK_SubscriptionInfo", "SubscriptionInfo", "Id");
            migrationBuilder.AddPrimaryKey("PK_TwitchSubscription", "TwitchSubscription", "Id");
            migrationBuilder.CreateIndex(
                name: "IX_TwitchSubscription_SubscriptionInfoId",
                table: "TwitchSubscription",
                column: "SubscriptionInfoId");
            migrationBuilder.AddForeignKey(name: "FK_TwitchSubscription_SubscriptionInfo_SubscriptionInfoId",
                column: "SubscriptionInfoId",
                principalTable: "SubscriptionInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade,
                table: "TwitchSubscription");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable("SubscriptionInfo", newName: "TrackedEvents");
            migrationBuilder.RenameTable("TwitchSubscription", newName: "TwitchEvents");
            
            migrationBuilder.RenameColumn("TwitchSubscription", "TwitchSubscription", "TrackedEventId");

            migrationBuilder.DropIndex("IX_TwitchSubscription_SubscriptionInfoId");
            
            migrationBuilder.DropPrimaryKey("PK_SubscriptionInfo", "TrackedEvents");
            migrationBuilder.AddPrimaryKey("PK_TrackedEvents", "TrackedEvents", "Id");

            migrationBuilder.DropPrimaryKey("PK_TwitchSubscription", "TwitchEvents");
            migrationBuilder.AddPrimaryKey("PK_TwitchEvents", "TwitchEvents", "Id");
            migrationBuilder.CreateIndex(
                name: "IX_TwitchEvents_TrackedEventId",
                table: "TwitchEvent",
                column: "TrackedEventsId");
        }
    }
}
