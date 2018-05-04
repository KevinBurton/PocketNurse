using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PocketNurse.Migrations
{
    public partial class ChangeKeyTypeMedicationOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicationOrder",
                table: "MedicationOrder");

            migrationBuilder.DropColumn(
                name: "MedicationId",
                table: "MedicationOrder");

            migrationBuilder.AddColumn<int>(
                name: "MedicationOrderId",
                table: "MedicationOrder",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicationOrder",
                table: "MedicationOrder",
                column: "MedicationOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicationOrder",
                table: "MedicationOrder");

            migrationBuilder.DropColumn(
                name: "MedicationOrderId",
                table: "MedicationOrder");

            migrationBuilder.AddColumn<Guid>(
                name: "MedicationId",
                table: "MedicationOrder",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicationOrder",
                table: "MedicationOrder",
                column: "MedicationId");
        }
    }
}
