namespace MsClean.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MsClean.Domain;

public class PermissionTypeConfiguration : EntityMapBase<PermissionType>
{
    protected override void Configure(EntityTypeBuilder<PermissionType> builder)
    {
        builder.ToTable("PermissionType");
        builder.Property(b => b.Description).HasMaxLength(255).IsRequired();
    }
}