using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquipmentBorrowing.Infrastructure.Persistence.Migrations
{
    public partial class SeedInitialData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "StudentId", "Name", "Program", "IsAuthorized" },
                values: new object[,]
                {
                    { "1", "John Doe", "Computer Science", true },
                    { "2", "Jane Smith", "Mathematics", false },
                    { "3", "Alice Johnson", "Physics", true }
                });

            migrationBuilder.InsertData(
                table: "Equipment",
                columns: new[] { "EquipmentId", "Name", "Type", "IsActivelyBorrowed" },
                values: new object[,]
                {
                    { "1", "Laptop", "Electronics", false },
                    { "2", "Projector", "Electronics", false },
                    { "3", "Camera", "Electronics", false }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var id in new[] { "1", "2", "3" })
            {
                migrationBuilder.DeleteData(
                    table: "Equipment",
                    keyColumn: "EquipmentId",
                    keyValue: id);

                migrationBuilder.DeleteData(
                    table: "Students",
                    keyColumn: "StudentId",
                    keyValue: id);
            }
        }
    }
}