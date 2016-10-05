using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MobileSecondHand.Migrations
{
    public partial class AddedCategoryModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "AdvertisementItem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementItem_CategoryId",
                table: "AdvertisementItem",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdvertisementItem_Category_CategoryId",
                table: "AdvertisementItem",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvertisementItem_Category_CategoryId",
                table: "AdvertisementItem");

            migrationBuilder.DropIndex(
                name: "IX_AdvertisementItem_CategoryId",
                table: "AdvertisementItem");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "AdvertisementItem");

            migrationBuilder.DropTable(
                name: "Category");
        }
    }
}
