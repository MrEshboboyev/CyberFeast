using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Messaging.Persistence.Postgres.MessagePersistence;

/// <summary>
/// Configures the entity type for the StoreMessage class.
/// </summary>
public class MessagePersistenceEntityTypeConfiguration : IEntityTypeConfiguration<StoreMessage>
{
    /// <summary>
    /// Configures the StoreMessage entity properties and mappings.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<StoreMessage> builder)
    {
        builder.ToTable("store_messages", MessagePersistenceDbContext.DefaultSchema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).IsRequired();

        builder.Property(x => x.RetryCount).HasColumnType("int").HasDefaultValue(0);

        builder
            .Property(x => x.DeliveryType)
            .HasMaxLength(50)
            .HasConversion(v => v.ToString(), v => Enum.Parse<MessageDeliveryType>(v))
            .IsRequired()
            .IsUnicode(false);

        builder
            .Property(x => x.MessageStatus)
            .HasMaxLength(50)
            .HasConversion(v => v.ToString(), v => Enum.Parse<MessageStatus>(v))
            .IsRequired()
            .IsUnicode(false);
    }
}