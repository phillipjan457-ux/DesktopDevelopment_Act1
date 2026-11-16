using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentBorrowing.Infrastructure.Persistence.Configurations;

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("Equipment", table =>
        {
            table.HasCheckConstraint(
                "CK_Equipment_EquipmentId_NotBlank",
                "length(trim(\"EquipmentId\")) > 0");

            table.HasCheckConstraint(
                "CK_Equipment_Name_NotBlank",
                "length(trim(\"Name\")) > 0");
        });

        builder.HasKey(equipment => equipment.EquipmentId);

        builder.Property(equipment => equipment.EquipmentId)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(equipment => equipment.Name)
            .IsRequired();

        builder.Property(equipment => equipment.Type)
            .IsRequired(false);

        builder.Property(equipment => equipment.Description)
            .IsRequired(false);

        builder.Property(equipment => equipment.IsActivelyBorrowed)
            .IsRequired()
            .HasDefaultValue(false);
    }
}