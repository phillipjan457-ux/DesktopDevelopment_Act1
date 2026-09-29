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
                "length(trim(EquipmentId)) > 0");

            table.HasCheckConstraint(
                "CK_Equipment_Name_NotBlank",
                "length(trim(Name)) > 0");

            table.HasCheckConstraint(
                "CK_Equipment_Type_NotBlank",
                "length(trim(Type)) > 0");

            table.HasCheckConstraint(
                "CK_Equipment_IsActivelyBorrowed",
                "IsActivelyBorrowed IN (0, 1)");
        });

        builder.HasKey(equipment => equipment.EquipmentId);

        builder.Property(equipment => equipment.EquipmentId)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(equipment => equipment.Name)
            .IsRequired();

        builder.Property(equipment => equipment.Type)
            .IsRequired();

        builder.Property(equipment => equipment.IsActivelyBorrowed)
            .IsRequired();
    }
}