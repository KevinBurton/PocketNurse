using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PocketNurse.Migrations
{
    public partial class AddPatientAllergy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Allergy_Patient_PatientId",
                table: "Allergy");

            migrationBuilder.DropIndex(
                name: "IX_Allergy_PatientId",
                table: "Allergy");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Allergy");

            migrationBuilder.CreateTable(
                name: "PatientAllergy",
                columns: table => new
                {
                    PatientAllergyId = table.Column<Guid>(nullable: false),
                    AllergyId = table.Column<Guid>(nullable: true),
                    PatientId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientAllergy", x => x.PatientAllergyId);
                    table.ForeignKey(
                        name: "FK_PatientAllergy_Allergy_AllergyId",
                        column: x => x.AllergyId,
                        principalTable: "Allergy",
                        principalColumn: "AllergyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientAllergy_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientAllergy_AllergyId",
                table: "PatientAllergy",
                column: "AllergyId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAllergy_PatientId",
                table: "PatientAllergy",
                column: "PatientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientAllergy");

            migrationBuilder.AddColumn<string>(
                name: "PatientId",
                table: "Allergy",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Allergy_PatientId",
                table: "Allergy",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Allergy_Patient_PatientId",
                table: "Allergy",
                column: "PatientId",
                principalTable: "Patient",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
