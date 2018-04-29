﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using PocketNurse.Models;
using System;

namespace PocketNurse.Migrations
{
    [DbContext(typeof(PocketNurseContext))]
    [Migration("20180429134815_AddCabinetSession")]
    partial class AddCabinetSession
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PocketNurse.Models.Allergy", b =>
                {
                    b.Property<Guid>("AllergyId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AllergyName")
                        .HasMaxLength(128);

                    b.HasKey("AllergyId");

                    b.ToTable("Allergy");
                });

            modelBuilder.Entity("PocketNurse.Models.Cabinet", b =>
                {
                    b.Property<string>("CabinetId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Area")
                        .HasMaxLength(32);

                    b.Property<string>("State")
                        .HasMaxLength(32);

                    b.HasKey("CabinetId");

                    b.ToTable("Cabinet");
                });

            modelBuilder.Entity("PocketNurse.Models.CabinetSession", b =>
                {
                    b.Property<string>("CabinetSessionId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CabinetId");

                    b.Property<DateTime>("TimeStamp");

                    b.HasKey("CabinetSessionId");

                    b.HasIndex("CabinetId");

                    b.ToTable("CabinetSession");
                });

            modelBuilder.Entity("PocketNurse.Models.MedicationOrder", b =>
                {
                    b.Property<Guid>("MedicationId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Dose")
                        .HasMaxLength(64);

                    b.Property<string>("Frequency")
                        .HasMaxLength(64);

                    b.Property<string>("MedicationName")
                        .HasMaxLength(128);

                    b.Property<string>("PatientId");

                    b.Property<string>("PocketNurseItemId")
                        .HasMaxLength(128);

                    b.Property<string>("Route")
                        .HasMaxLength(64);

                    b.HasKey("MedicationId");

                    b.HasIndex("PatientId");

                    b.ToTable("MedicationOrder");
                });

            modelBuilder.Entity("PocketNurse.Models.NotInFormulary", b =>
                {
                    b.Property<int>("_id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Alias");

                    b.Property<string>("GenericName");

                    b.Property<string>("Route");

                    b.Property<string>("Strength");

                    b.Property<string>("StrengthUnit");

                    b.Property<string>("TotalContainerVolume");

                    b.Property<string>("Volume");

                    b.Property<string>("VolumeUnit");

                    b.HasKey("_id");

                    b.ToTable("NotInFormulary");
                });

            modelBuilder.Entity("PocketNurse.Models.Patient", b =>
                {
                    b.Property<string>("PatientId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DOB");

                    b.Property<string>("First")
                        .HasMaxLength(128);

                    b.Property<string>("FullName")
                        .HasMaxLength(255);

                    b.Property<string>("Last")
                        .HasMaxLength(128);

                    b.Property<string>("MRN");

                    b.HasKey("PatientId");

                    b.ToTable("Patient");
                });

            modelBuilder.Entity("PocketNurse.Models.PatientAllergy", b =>
                {
                    b.Property<Guid>("PatientAllergyId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("AllergyId");

                    b.Property<string>("PatientId");

                    b.HasKey("PatientAllergyId");

                    b.HasIndex("AllergyId");

                    b.HasIndex("PatientId");

                    b.ToTable("PatientAllergy");
                });

            modelBuilder.Entity("PocketNurse.Models.CabinetSession", b =>
                {
                    b.HasOne("PocketNurse.Models.Cabinet", "Cabinet")
                        .WithMany()
                        .HasForeignKey("CabinetId");
                });

            modelBuilder.Entity("PocketNurse.Models.MedicationOrder", b =>
                {
                    b.HasOne("PocketNurse.Models.Patient", "Patient")
                        .WithMany()
                        .HasForeignKey("PatientId");
                });

            modelBuilder.Entity("PocketNurse.Models.PatientAllergy", b =>
                {
                    b.HasOne("PocketNurse.Models.Allergy", "Allergy")
                        .WithMany()
                        .HasForeignKey("AllergyId");

                    b.HasOne("PocketNurse.Models.Patient", "Patient")
                        .WithMany()
                        .HasForeignKey("PatientId");
                });
#pragma warning restore 612, 618
        }
    }
}
