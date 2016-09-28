using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MobileSecondHand.Migrations
{
    public partial class AddedUserToFavouritesAdvertisementsModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserToFavouriteAdvertisement",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(nullable: false),
                    AdvertisementItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToFavouriteAdvertisement", x => new { x.ApplicationUserId, x.AdvertisementItemId });
                    table.ForeignKey(
                        name: "FK_UserToFavouriteAdvertisement_AdvertisementItem_AdvertisementItemId",
                        column: x => x.AdvertisementItemId,
                        principalTable: "AdvertisementItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserToFavouriteAdvertisement_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserToFavouriteAdvertisement_AdvertisementItemId",
                table: "UserToFavouriteAdvertisement",
                column: "AdvertisementItemId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToFavouriteAdvertisement_ApplicationUserId",
                table: "UserToFavouriteAdvertisement",
                column: "ApplicationUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserToFavouriteAdvertisement");
        }
    }
}
