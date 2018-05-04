using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PocketNurse.Migrations
{
    public partial class RelateCabinetPatient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "CabinetSessionId",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "PatientCabinetSessionId",
                table: "MedicationOrder");

            migrationBuilder.AddColumn<int>(
                name: "PatientCabinetId",
                table: "PatientAllergy",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CabinetId",
                table: "Patient",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PatientCabinetId",
                table: "MedicationOrder",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patient",
                table: "Patient",
                columns: new[] { "PatientId", "CabinetId" });

            migrationBuilder.CreateIndex(
                name: "IX_PatientAllergy_PatientId_PatientCabinetId",
                table: "PatientAllergy",
                columns: new[] { "PatientId", "PatientCabinetId" });

            migrationBuilder.CreateIndex(
                name: "IX_Patient_CabinetId",
                table: "Patient",
                column: "CabinetId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationOrder_PatientId_PatientCabinetId",
                table: "MedicationOrder",
                columns: new[] { "PatientId", "PatientCabinetId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationOrder_Patient_PatientId_PatientCabinetId",
                table: "MedicationOrder",
                columns: new[] { "PatientId", "PatientCabinetId" },
                principalTable: "Patient",
                principalColumns: new[] { "PatientId", "CabinetId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patient_Cabinet_CabinetId",
                table: "Patient",
                column: "CabinetId",
                principalTable: "Cabinet",
                principalColumn: "CabinetId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientAllergy_Patient_PatientId_PatientCabinetId",
                table: "PatientAllergy",
                columns: new[] { "PatientId", "PatientCabinetId" },
                principalTable: "Patient",
                principalColumns: new[] { "PatientId", "CabinetId" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicationOrder_Patient_PatientId_PatientCabinetId",
                table: "MedicationOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_Patient_Cabinet_CabinetId",
                table: "Patient");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientAllergy_Patient_PatientId_PatientCabinetId",
                table: "PatientAllergy");

            migrationBuilder.DropIndex(
                name: "IX_PatientAllergy_PatientId_PatientCabinetId",
                table: "PatientAllergy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patient",
                table: "Patient");

            migrationBuilder.DropIndex(
                name: "IX_Patient_CabinetId",
                table: "Patient");

            migrationBuilder.DropIndex(
                name: "IX_MedicationOrder_PatientId_PatientCabinetId",
                table: "MedicationOrder");

            migrationBuilder.DropColumn(
                name: "PatientCabinetId",
                table: "PatientAllergy");

            migrationBuilder.DropColumn(
                name: "CabinetId",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "PatientCabinetId",
                table: "MedicationOrder");

            migrationBuilder.AddColumn<string>(
                name: "PatientCabinetSessionId",
                table: "PatientAllergy",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CabinetSessionId",
                table: "Patient",
                maxLength: 450,
                nullable: false,
                defaultValueSql: "-1");

            migrationBuilder.AddColumn<string>(
                name: "PatientCabinetSessionId",
                table: "MedicationOrder",
                nullable: true);

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
    }
}
