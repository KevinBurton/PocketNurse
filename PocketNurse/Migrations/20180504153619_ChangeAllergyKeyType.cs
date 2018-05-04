using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PocketNurse.Migrations
{
    public partial class ChangeAllergyKeyType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PatientAllergy_AllergyId",
                table: "PatientAllergy");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientAllergy_Allergy_AllergyId",
                table: "PatientAllergy");

            migrationBuilder.DropColumn(
                name: "AllergyId",
                table: "PatientAllergy");

            migrationBuilder.AddColumn<int>(
                name: "AllergyId",
                table: "PatientAllergy",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PatientAllergy_AllergyId",
                table: "PatientAllergy",
                column: "AllergyId");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Allergy",
                table: "Allergy");

            migrationBuilder.DropColumn(
                name: "AllergyId",
                table: "Allergy");

            migrationBuilder.AddColumn<int>(
                name: "AllergyId",
                table: "Allergy",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Allergy",
                table: "Allergy",
                column: "AllergyId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientAllergy_Allergy_AllergyId",
                table: "PatientAllergy",
                column: "AllergyId",
                principalTable: "Allergy",
                principalColumn: "AllergyId");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "AllergyId",
                table: "PatientAllergy",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AllergyId",
                table: "Allergy",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
        }
    }
}
