using CareFlow.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareFlow.Infrastructure.Migrations;

[DbContext(typeof(CareFlowDbContext))]
[Migration("20261120150000_InitialCareFlow")]
public sealed class InitialCareFlow : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "DiagnosticServices",
            columns: table => new
            {
                DiagnosticServiceId = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                DailyCapacity = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_DiagnosticServices", x => x.DiagnosticServiceId); table.CheckConstraint("CK_DiagnosticServices_Capacity", "DailyCapacity > 0"); });

        migrationBuilder.CreateTable(
            name: "Patients",
            columns: table => new
            {
                PatientId = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                FullName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Patients", x => x.PatientId));

        migrationBuilder.CreateTable(
            name: "Bookings",
            columns: table => new
            {
                BookingId = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                PatientId = table.Column<int>(type: "INTEGER", nullable: false),
                BookingDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                BookedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Bookings", x => x.BookingId);
                table.ForeignKey("FK_Bookings_Patients_PatientId", x => x.PatientId, "Patients", "PatientId", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "BookingServices",
            columns: table => new
            {
                BookingId = table.Column<int>(type: "INTEGER", nullable: false),
                DiagnosticServiceId = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BookingServices", x => new { x.BookingId, x.DiagnosticServiceId });
                table.ForeignKey("FK_BookingServices_Bookings_BookingId", x => x.BookingId, "Bookings", "BookingId", onDelete: ReferentialAction.Cascade);
                table.ForeignKey("FK_BookingServices_DiagnosticServices_DiagnosticServiceId", x => x.DiagnosticServiceId, "DiagnosticServices", "DiagnosticServiceId", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex("IX_BookingServices_DiagnosticServiceId", "BookingServices", "DiagnosticServiceId");
        migrationBuilder.CreateIndex("IX_Bookings_BookingDate_PatientId", "Bookings", new[] { "BookingDate", "PatientId" });
        migrationBuilder.CreateIndex("IX_DiagnosticServices_Name", "DiagnosticServices", "Name", unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("BookingServices");
        migrationBuilder.DropTable("Bookings");
        migrationBuilder.DropTable("DiagnosticServices");
        migrationBuilder.DropTable("Patients");
    }
}
