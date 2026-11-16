using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentBorrowing.Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students", table =>
        {
            table.HasCheckConstraint(
                "CK_Students_StudentId_NotBlank",
                "length(trim(\"StudentId\")) > 0");

            table.HasCheckConstraint(
                "CK_Students_Name_NotBlank",
                "length(trim(\"Name\")) > 0");
        });

        builder.HasKey(student => student.StudentId);

        builder.Property(student => student.StudentId)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(student => student.Name)
            .IsRequired();

        builder.Property(student => student.Program)
            .IsRequired(false);

        builder.Property(student => student.Contact)
            .IsRequired(false);

        builder.Property(student => student.IsAuthorized)
            .IsRequired()
            .HasDefaultValue(true);
    }
}