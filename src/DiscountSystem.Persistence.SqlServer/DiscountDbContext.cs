using DiscountSystem.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DiscountSystem.Persistence.SqlServer;

public class DiscountDbContext : DbContext
{
    public DiscountDbContext(DbContextOptions<DiscountDbContext> options) : base(options) { }

    public DbSet<DiscountCodeTable> DiscountCodes => Set<DiscountCodeTable>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var codeConverter = new ValueConverter<Code, string>(
            v => v.Value,
            v => new Code(v));

        modelBuilder.Entity<DiscountCodeTable>(b =>
        {
            b.ToTable("DiscountCodes", "dbo");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id)
                .HasConversion(codeConverter)
                .HasColumnType("varchar(8)")
                .IsRequired();

            b.Property(x => x.Status)
                .HasConversion<byte>()
                .HasColumnType("tinyint")
                .IsRequired();

            b.Property(x => x.CreatedAtUtc)
                .HasColumnType("datetime2")
                .IsRequired();

            b.Property(x => x.UsedAtUtc)
                .HasColumnType("datetime2");

            // optimistic concurrency:
            b.Property<byte[]>("RowVersion").IsRowVersion().IsConcurrencyToken();
        });
    }
}