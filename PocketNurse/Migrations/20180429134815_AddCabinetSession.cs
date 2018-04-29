using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PocketNurse.Migrations
{
    public partial class AddCabinetSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cabinet",
                columns: table => new
                {
                    CabinetId = table.Column<string>(nullable: false),
                    Area = table.Column<string>(maxLength: 32, nullable: true),
                    State = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cabinet", x => x.CabinetId);
                });

            migrationBuilder.CreateTable(
                name: "CabinetSession",
                columns: table => new
                {
                    CabinetSessionId = table.Column<string>(nullable: false),
                    CabinetId = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CabinetSession", x => x.CabinetSessionId);
                    table.ForeignKey(
                        name: "FK_CabinetSession_Cabinet_CabinetId",
                        column: x => x.CabinetId,
                        principalTable: "Cabinet",
                        principalColumn: "CabinetId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CabinetSession_CabinetId",
                table: "CabinetSession",
                column: "CabinetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CabinetSession");

            migrationBuilder.DropTable(
                name: "Cabinet");
        }
    }
}
