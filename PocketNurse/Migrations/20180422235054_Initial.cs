using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PocketNurse.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotInFormulary",
                columns: table => new
                {
                    _id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Alias = table.Column<string>(nullable: true),
                    GenericName = table.Column<string>(nullable: true),
                    Route = table.Column<string>(nullable: true),
                    Strength = table.Column<int>(nullable: false),
                    StrengthUnit = table.Column<string>(nullable: true),
                    TotalContainerVolume = table.Column<string>(nullable: true),
                    Volume = table.Column<int>(nullable: false),
                    VolumeUnit = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotInFormulary", x => x._id);
                });

            migrationBuilder.CreateTable(
                name: "Patient",
                columns: table => new
                {
                    PatientId = table.Column<string>(nullable: false),
                    DOB = table.Column<DateTime>(nullable: false),
                    First = table.Column<string>(maxLength: 128, nullable: true),
                    FullName = table.Column<string>(maxLength: 255, nullable: true),
                    Last = table.Column<string>(maxLength: 128, nullable: true),
                    MRN = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient", x => x.PatientId);
                });

            migrationBuilder.CreateTable(
                name: "Allergy",
                columns: table => new
                {
                    AllergyId = table.Column<Guid>(nullable: false),
                    AllergyName = table.Column<string>(maxLength: 128, nullable: true),
                    PatientId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allergy", x => x.AllergyId);
                    table.ForeignKey(
                        name: "FK_Allergy_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicationOrder",
                columns: table => new
                {
                    MedicationId = table.Column<Guid>(nullable: false),
                    Dose = table.Column<string>(maxLength: 64, nullable: true),
                    Frequency = table.Column<string>(maxLength: 64, nullable: true),
                    MedicationName = table.Column<string>(maxLength: 128, nullable: true),
                    PatientId = table.Column<string>(nullable: true),
                    Route = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationOrder", x => x.MedicationId);
                    table.ForeignKey(
                        name: "FK_MedicationOrder_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Allergy_PatientId",
                table: "Allergy",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationOrder_PatientId",
                table: "MedicationOrder",
                column: "PatientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Allergy");

            migrationBuilder.DropTable(
                name: "MedicationOrder");

            migrationBuilder.DropTable(
                name: "NotInFormulary");

            migrationBuilder.DropTable(
                name: "Patient");
        }
    }
}
