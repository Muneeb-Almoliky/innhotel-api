using InnHotel.Core.GuestAggregate;
using InnHotel.Core.GuestAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InnHotel.Infrastructure.Data.Config;

public class GuestConfiguration : IEntityTypeConfiguration<Guest>
{
  public void Configure(EntityTypeBuilder<Guest> g)
  {
    g.ToTable("guests");

    g.HasKey(x => x.Id)
     .HasName("guest_id");

    g.Property(x => x.FirstName)
     .HasColumnName("first_name")
     .IsRequired()
     .HasMaxLength(50);

    g.Property(x => x.LastName)
     .HasColumnName("last_name")
     .IsRequired()
     .HasMaxLength(50);

    g.Property(x => x.Gender)
     .HasColumnName("gender")
     .HasConversion<string>() // تحويل إلى string لتخزينها في PostgreSQL
     .IsRequired()
     .HasDefaultValue(Gender.Male);

    g.Property(x => x.IdProofType)
     .HasColumnName("id_proof_type")
     .HasConversion<string>()
     .IsRequired();

    g.Property(x => x.IdProofNumber)
     .HasColumnName("id_proof_number")
     .IsRequired()
     .HasMaxLength(50);

    g.HasIndex(x => x.IdProofNumber)
     .IsUnique()
     .HasDatabaseName("UX_guests_idproof_number");

    g.Property(x => x.Email)
     .HasColumnName("email")
     .HasMaxLength(100);

    g.Property(x => x.Phone)
     .HasColumnName("phone")
     .HasMaxLength(20);

    g.Property(x => x.Address)
     .HasColumnName("address")
     .HasMaxLength(250);
  }
}
