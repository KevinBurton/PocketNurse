using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PocketNurse.Migrations
{
    public partial class ConstrainPatientCabinetSessionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CabinetSessionId",
                table: "Patient",
                maxLength: 450,
                nullable: false,
                defaultValueSql: "-1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CabinetSessionId",
                table: "Patient");
        }
    }
}
