namespace MsClean.Infrastructure;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public abstract class EntityMapBase<T> : IEntityTypeConfiguration<T> where T : Entity
{
    void IEntityTypeConfiguration<T>.Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).UseIdentityColumn();
        builder.Property(b => b.UserRegister).HasMaxLength(100).IsRequired();
        builder.Property(b => b.UserUpdated).HasMaxLength(100).IsRequired(false);
        builder.Property(b => b.DateTimeRegister).IsRequired();
        builder.Property(b => b.DateTimeUpdated).IsRequired(false);
        builder.Property(b => b.IsActive).HasDefaultValue(true);
        Configure(builder);
    }

    protected abstract void Configure(EntityTypeBuilder<T> builder);
}
