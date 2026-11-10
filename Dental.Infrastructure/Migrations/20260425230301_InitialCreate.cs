using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dental.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DentalHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastDentalVisitDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastDentalVisitReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasGumBleeding = table.Column<bool>(type: "bit", nullable: false),
                    HasToothPain = table.Column<bool>(type: "bit", nullable: false),
                    HasHotColdSensitivity = table.Column<bool>(type: "bit", nullable: false),
                    HasBruxism = table.Column<bool>(type: "bit", nullable: false),
                    HasJawPain = table.Column<bool>(type: "bit", nullable: false),
                    HadOrthodonticTreatment = table.Column<bool>(type: "bit", nullable: false),
                    HasCrownsOrBridges = table.Column<bool>(type: "bit", nullable: false),
                    HasPreviousFillings = table.Column<bool>(type: "bit", nullable: false),
                    HadToothExtraction = table.Column<bool>(type: "bit", nullable: false),
                    HadRootCanalTreatment = table.Column<bool>(type: "bit", nullable: false),
                    HasGumDisease = table.Column<bool>(type: "bit", nullable: false),
                    OralHygieneHabits = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DentalHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DentalHistories_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicalHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HasHypertension = table.Column<bool>(type: "bit", nullable: false),
                    HasDiabetes = table.Column<bool>(type: "bit", nullable: false),
                    HasThyroidDisorder = table.Column<bool>(type: "bit", nullable: false),
                    HasAllergy = table.Column<bool>(type: "bit", nullable: false),
                    HasHeartDisease = table.Column<bool>(type: "bit", nullable: false),
                    HasBleedingDisorder = table.Column<bool>(type: "bit", nullable: false),
                    HasKidneyDisease = table.Column<bool>(type: "bit", nullable: false),
                    HasLiverDisease = table.Column<bool>(type: "bit", nullable: false),
                    HasCancer = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalHistories_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teeth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ToothNumber = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Arch = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teeth", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teeth_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ToothRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Treatment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CanalLength = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToothId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToothRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToothRecords_Teeth_ToothId",
                        column: x => x.ToothId,
                        principalTable: "Teeth",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ToothImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageType = table.Column<int>(type: "int", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToothRecordId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToothImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToothImages_ToothRecords_ToothRecordId",
                        column: x => x.ToothRecordId,
                        principalTable: "ToothRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DentalHistories_PatientId",
                table: "DentalHistories",
                column: "PatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicalHistories_PatientId",
                table: "MedicalHistories",
                column: "PatientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teeth_PatientId",
                table: "Teeth",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ToothImages_ToothRecordId",
                table: "ToothImages",
                column: "ToothRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ToothRecords_ToothId",
                table: "ToothRecords",
                column: "ToothId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DentalHistories");

            migrationBuilder.DropTable(
                name: "MedicalHistories");

            migrationBuilder.DropTable(
                name: "ToothImages");

            migrationBuilder.DropTable(
                name: "ToothRecords");

            migrationBuilder.DropTable(
                name: "Teeth");

            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
