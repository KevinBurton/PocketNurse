using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PocketNurse.Migrations
{
    public partial class ChangeCabinetPKType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CabinetSession_Cabinet_CabinetId",
                table: "CabinetSession");
            migrationBuilder.DropIndex(
                name: "IX_CabinetSession_CabinetId",
                table: "CabinetSession");
            migrationBuilder.DropColumn(
                name: "CabinetId",
                table: "CabinetSession");
            migrationBuilder.AddColumn<int>(
                name: "CabinetId",
                table: "CabinetSession",
                nullable: true);
            migrationBuilder.CreateIndex(
                name: "IX_CabinetSession_CabinetId",
                table: "CabinetSession",
                column: "CabinetId");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cabinet",
                table: "Cabinet");
            migrationBuilder.DropColumn(
                name: "CabinetId",
                table: "Cabinet");
            migrationBuilder.AddColumn<int>(
                name: "CabinetId",
                table: "Cabinet",
                nullable: false)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
            migrationBuilder.AddPrimaryKey(
                name: "PK_Cabinet",
                table: "Cabinet",
                column: "CabinetId");

            migrationBuilder.AddForeignKey(
                name: "FK_CabinetSession_Cabinet_CabinetId",
                table: "CabinetSession",
                column: "CabinetId",
                principalTable: "Cabinet",
                principalColumn: "CabinetId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CabinetId",
                table: "CabinetSession",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CabinetId",
                table: "Cabinet",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
        }
    }
}
