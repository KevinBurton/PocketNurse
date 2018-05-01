using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PocketNurse.Migrations
{
    public partial class CompositePatientIdCabinetSessionIdKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicationOrder_Patient_PatientId",
                table: "MedicationOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientAllergy_Patient_PatientId",
                table: "PatientAllergy");

            migrationBuilder.DropIndex(
                name: "IX_PatientAllergy_PatientId",
                table: "PatientAllergy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patient",
                table: "Patient");

            migrationBuilder.DropIndex(
                name: "IX_MedicationOrder_PatientId",
                table: "MedicationOrder");

            migrationBuilder.AddColumn<string>(
                name: "PatientCabinetSessionId",
                table: "PatientAllergy",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatientCabinetSessionId",
                table: "MedicationOrder",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Patient_PatientId",
                table: "Patient",
                column: "PatientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patient",
                table: "Patient",
                columns: new[] { "PatientId", "CabinetSessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_PatientAllergy_PatientId_PatientCabinetSessionId",
                table: "PatientAllergy",
                columns: new[] { "PatientId", "PatientCabinetSessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_MedicationOrder_PatientId_PatientCabinetSessionId",
                table: "MedicationOrder",
                columns: new[] { "PatientId", "PatientCabinetSessionId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationOrder_Patient_PatientId_PatientCabinetSessionId",
                table: "MedicationOrder",
                columns: new[] { "PatientId", "PatientCabinetSessionId" },
                principalTable: "Patient",
                principalColumns: new[] { "PatientId", "CabinetSessionId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientAllergy_Patient_PatientId_PatientCabinetSessionId",
                table: "PatientAllergy",
                columns: new[] { "PatientId", "PatientCabinetSessionId" },
                principalTable: "Patient",
                principalColumns: new[] { "PatientId", "CabinetSessionId" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicationOrder_Patient_PatientId_PatientCabinetSessionId",
                table: "MedicationOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientAllergy_Patient_PatientId_PatientCabinetSessionId",
                table: "PatientAllergy");

            migrationBuilder.DropIndex(
                name: "IX_PatientAllergy_PatientId_PatientCabinetSessionId",
                table: "PatientAllergy");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Patient_PatientId",
                table: "Patient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patient",
                table: "Patient");

            migrationBuilder.DropIndex(
                name: "IX_MedicationOrder_PatientId_PatientCabinetSessionId",
                table: "MedicationOrder");

            migrationBuilder.DropColumn(
                name: "PatientCabinetSessionId",
                table: "PatientAllergy");

            migrationBuilder.DropColumn(
                name: "PatientCabinetSessionId",
                table: "MedicationOrder");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patient",
                table: "Patient",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAllergy_PatientId",
                table: "PatientAllergy",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationOrder_PatientId",
                table: "MedicationOrder",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationOrder_Patient_PatientId",
                table: "MedicationOrder",
                column: "PatientId",
                principalTable: "Patient",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientAllergy_Patient_PatientId",
                table: "PatientAllergy",
                column: "PatientId",
                principalTable: "Patient",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
