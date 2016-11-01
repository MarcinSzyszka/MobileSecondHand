using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MobileSecondHand.Migrations
{
    public partial class AddedWrongAdvertisementIssueModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AdvertisementItem");

            migrationBuilder.CreateTable(
                name: "WrongAdvertisementIssue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdvertisementId = table.Column<int>(nullable: false),
                    ConsideredByUserId = table.Column<string>(nullable: true),
                    IsConsidered = table.Column<bool>(nullable: false),
                    IssueAuthorId = table.Column<string>(nullable: true),
                    Reason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WrongAdvertisementIssue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WrongAdvertisementIssue_AdvertisementItem_AdvertisementId",
                        column: x => x.AdvertisementId,
                        principalTable: "AdvertisementItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WrongAdvertisementIssue_AspNetUsers_ConsideredByUserId",
                        column: x => x.ConsideredByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WrongAdvertisementIssue_AspNetUsers_IssueAuthorId",
                        column: x => x.IssueAuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddColumn<bool>(
                name: "IsBlockedByAdmin",
                table: "AdvertisementItem",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_WrongAdvertisementIssue_AdvertisementId",
                table: "WrongAdvertisementIssue",
                column: "AdvertisementId");

            migrationBuilder.CreateIndex(
                name: "IX_WrongAdvertisementIssue_ConsideredByUserId",
                table: "WrongAdvertisementIssue",
                column: "ConsideredByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WrongAdvertisementIssue_IssueAuthorId",
                table: "WrongAdvertisementIssue",
                column: "IssueAuthorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlockedByAdmin",
                table: "AdvertisementItem");

            migrationBuilder.DropTable(
                name: "WrongAdvertisementIssue");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AdvertisementItem",
                nullable: false,
                defaultValue: false);
        }
    }
}
