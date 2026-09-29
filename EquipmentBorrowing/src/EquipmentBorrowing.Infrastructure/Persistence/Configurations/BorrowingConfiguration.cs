using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentBorrowing.Infrastructure.Persistence.Configurations;

public class BorrowingConfiguration : IEntityTypeConfiguration<Borrowing>
{
    public void Configure(EntityTypeBuilder<Borrowing> builder)
    {
        builder.ToTable("Borrowings", table =>
        {
            table.HasCheckConstraint(
                "CK_Borrowings_BorrowId_NotBlank",
                "length(trim(BorrowId)) > 0");

            table.HasCheckConstraint(
                "CK_Borrowings_Status",
                "Status IN (0, 1)");

            table.HasCheckConstraint(
                "CK_Borrowings_ReturnDate",
                "ReturnDate > BorrowDate");

            table.HasCheckConstraint(
                "CK_Borrowings_ReturnedAt",
                "ReturnedAt IS NULL OR ReturnedAt >= BorrowDate");

            table.HasCheckConstraint(
                "CK_Borrowings_Status_ReturnedAt",
                "(Status = 0 AND ReturnedAt IS NULL) OR " +
                "(Status = 1 AND ReturnedAt IS NOT NULL)");
        });

        builder.HasKey(borrowing => borrowing.BorrowId);

        builder.Property(borrowing => borrowing.BorrowId)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(borrowing => borrowing.StudentId)
            .IsRequired();

        builder.Property(borrowing => borrowing.EquipmentId)
            .IsRequired();

        builder.Property(borrowing => borrowing.BorrowDate)
            .IsRequired();

        builder.Property(borrowing => borrowing.ReturnDate)
            .IsRequired();

        builder.Property(borrowing => borrowing.ReturnedAt)
            .IsRequired(false);

        builder.Property(borrowing => borrowing.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(borrowing => borrowing.Student)
            .WithMany()
            .HasForeignKey(borrowing => borrowing.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(borrowing => borrowing.Equipment)
            .WithMany()
            .HasForeignKey(borrowing => borrowing.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(borrowing => new
        {
            borrowing.StudentId,
            borrowing.Status
        });

        builder.HasIndex(borrowing => borrowing.Status);

        builder.HasIndex(
            borrowing => borrowing.EquipmentId,
            "IX_Borrowings_EquipmentId");

        builder.HasIndex(
                borrowing => borrowing.EquipmentId,
                "UX_Borrowings_EquipmentId_Active")
            .IsUnique()
            .HasFilter("Status = 0");
    }
}