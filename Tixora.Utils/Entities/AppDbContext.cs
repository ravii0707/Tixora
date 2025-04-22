using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Tixora.Core.Entities;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbBookingHistory> TbBookingHistories { get; set; }

    public virtual DbSet<TbMovie> TbMovies { get; set; }

    public virtual DbSet<TbShowTime> TbShowTimes { get; set; }

    public virtual DbSet<TbUser> TbUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TbBookingHistory>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__tb_Booki__73951ACDFA672FD8");

            entity.Property(e => e.BookingDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Movie).WithMany(p => p.TbBookingHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Bookin__Movie__5629CD9C");

            entity.HasOne(d => d.Showtime).WithMany(p => p.TbBookingHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Bookin__Showt__5535A963");

            entity.HasOne(d => d.User).WithMany(p => p.TbBookingHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_Bookin__UserI__5441852A");
        });

        modelBuilder.Entity<TbMovie>(entity =>
        {
            entity.HasKey(e => e.MovieId).HasName("PK__tb_Movie__4BD2943A44FDE69D");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<TbShowTime>(entity =>
        {
            entity.HasKey(e => e.ShowtimeId).HasName("PK__tb_ShowT__32D31FC0161C36F6");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Movie).WithMany(p => p.TbShowTimes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_ShowTi__Movie__5070F446");
        });

        modelBuilder.Entity<TbUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__tb_Users__1788CCACC8DA8FAB");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
