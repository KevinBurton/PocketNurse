using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PocketNurse.Migrations
{
    public partial class ChangePatientAllergyKeyType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PatientAllergy",
                table: "PatientAllergy");

            migrationBuilder.DropColumn(
                name: "PatientAllergyId",
                table: "PatientAllergy");

            migrationBuilder.AddColumn<int>(
                name: "PatientAllergyId",
                table: "PatientAllergy",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PatientAllergy",
                table: "PatientAllergy",
                column: "PatientAllergyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "PatientAllergyId",
                table: "PatientAllergy",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
        }
    }
}
