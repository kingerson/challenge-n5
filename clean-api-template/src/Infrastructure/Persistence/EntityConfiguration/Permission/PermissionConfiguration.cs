namespace MsClean.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MsClean.Domain;

public class PermissionConfiguration : EntityMapBase<Permission>
{
    protected override void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permission");
        builder.Property(b => b.EmployeeForename).HasMaxLength(100).IsRequired();
        builder.Property(b => b.EmployeeLastName).HasMaxLength(100).IsRequired();
        builder.Property(b => b.PermissionDate).IsRequired();

        builder.HasOne(p => p.PermissionType)
            .WithMany(pt => pt.Permissions)
            .HasForeignKey(p => p.PermissionTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}