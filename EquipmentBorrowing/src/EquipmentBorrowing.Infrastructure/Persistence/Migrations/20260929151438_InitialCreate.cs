using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquipmentBorrowing.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    EquipmentId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    IsActivelyBorrowed = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.EquipmentId);
                    table.CheckConstraint("CK_Equipment_EquipmentId_NotBlank", "length(trim(EquipmentId)) > 0");
                    table.CheckConstraint("CK_Equipment_IsActivelyBorrowed", "IsActivelyBorrowed IN (0, 1)");
                    table.CheckConstraint("CK_Equipment_Name_NotBlank", "length(trim(Name)) > 0");
                    table.CheckConstraint("CK_Equipment_Type_NotBlank", "length(trim(Type)) > 0");
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Program = table.Column<string>(type: "TEXT", nullable: false),
                    IsAuthorized = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentId);
                    table.CheckConstraint("CK_Students_IsAuthorized", "IsAuthorized IN (0, 1)");
                    table.CheckConstraint("CK_Students_Name_NotBlank", "length(trim(Name)) > 0");
                    table.CheckConstraint("CK_Students_Program_NotBlank", "length(trim(Program)) > 0");
                    table.CheckConstraint("CK_Students_StudentId_NotBlank", "length(trim(StudentId)) > 0");
                });

            migrationBuilder.CreateTable(
                name: "Borrowings",
                columns: table => new
                {
                    BorrowId = table.Column<string>(type: "TEXT", nullable: false),
                    StudentId = table.Column<string>(type: "TEXT", nullable: false),
                    EquipmentId = table.Column<string>(type: "TEXT", nullable: false),
                    BorrowDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReturnedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Borrowings", x => x.BorrowId);
                    table.CheckConstraint("CK_Borrowings_BorrowId_NotBlank", "length(trim(BorrowId)) > 0");
                    table.CheckConstraint("CK_Borrowings_ReturnDate", "ReturnDate > BorrowDate");
                    table.CheckConstraint("CK_Borrowings_ReturnedAt", "ReturnedAt IS NULL OR ReturnedAt >= BorrowDate");
                    table.CheckConstraint("CK_Borrowings_Status", "Status IN (0, 1)");
                    table.CheckConstraint("CK_Borrowings_Status_ReturnedAt", "(Status = 0 AND ReturnedAt IS NULL) OR (Status = 1 AND ReturnedAt IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_Borrowings_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "EquipmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Borrowings_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Borrowings_EquipmentId",
                table: "Borrowings",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Borrowings_Status",
                table: "Borrowings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Borrowings_StudentId_Status",
                table: "Borrowings",
                columns: new[] { "StudentId", "Status" });

            migrationBuilder.CreateIndex(
                name: "UX_Borrowings_EquipmentId_Active",
                table: "Borrowings",
                column: "EquipmentId",
                unique: true,
                filter: "Status = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Borrowings");

            migrationBuilder.DropTable(
                name: "Equipment");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
