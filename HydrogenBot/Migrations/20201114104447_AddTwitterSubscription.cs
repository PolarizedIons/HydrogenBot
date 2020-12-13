using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HydrogenBot.Migrations
{
    public partial class AddTwitterSubscription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TwitterSubscription",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    SubscriptionInfoId = table.Column<Guid>(nullable: false),
                    Username = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwitterSubscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TwitterSubscription_SubscriptionInfo_SubscriptionInfoId",
                        column: x => x.SubscriptionInfoId,
                        principalTable: "SubscriptionInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TwitterSubscription_SubscriptionInfoId",
                table: "TwitterSubscription",
                column: "SubscriptionInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TwitterSubscription");
        }
    }
}
