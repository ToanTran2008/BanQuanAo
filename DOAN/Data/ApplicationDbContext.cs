using System;
using System.Collections.Generic;
using DOAN.Models;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cthoadon> Cthoadons { get; set; }

    public virtual DbSet<Cuahang> Cuahangs { get; set; }

    public virtual DbSet<Danhgium> Danhgia { get; set; }

    public virtual DbSet<Danhmuc> Danhmucs { get; set; }

    public virtual DbSet<Diachi> Diachis { get; set; }

    public virtual DbSet<Hoadon> Hoadons { get; set; }

    public virtual DbSet<Khuyenmai> Khuyenmais { get; set; }

    public virtual DbSet<Lichsumuahang> Lichsumuahangs { get; set; }

    public virtual DbSet<Mathang> Mathangs { get; set; }

    public virtual DbSet<Nguoidung> Nguoidungs { get; set; }

    public virtual DbSet<Phuongthucthanhtoan> Phuongthucthanhtoans { get; set; }

    public virtual DbSet<ThongkeDoanhthu> ThongkeDoanhthus { get; set; }

    public virtual DbSet<ThongkeMathangBan> ThongkeMathangBans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=TOANTRAN\\SQLEXPRESS;Initial Catalog=SHOPQUANAO;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cthoadon>(entity =>
        {
            entity.HasKey(e => e.MaCthd).HasName("PK__CTHOADON__1E4FA77100A0F24C");

            entity.Property(e => e.DonGia).HasDefaultValue(0);
            entity.Property(e => e.SoLuong).HasDefaultValue(1);
            entity.Property(e => e.ThanhTien).HasComputedColumnSql("([DonGia]*[SoLuong])", false);

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.Cthoadons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTHOADON__MaHD__6754599E");

            entity.HasOne(d => d.MaMhNavigation).WithMany(p => p.Cthoadons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTHOADON__MaMH__68487DD7");
        });

        modelBuilder.Entity<Cuahang>(entity =>
        {
            entity.HasKey(e => e.MaCh).HasName("PK__CUAHANG__27258E00221F416E");
        });

        modelBuilder.Entity<Danhgium>(entity =>
        {
            entity.HasKey(e => e.MaDg).HasName("PK__DANHGIA__27258660422EDE2E");

            entity.Property(e => e.NgayDanhGia).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaMhNavigation).WithMany(p => p.Danhgia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DANHGIA__MaMH__6D0D32F4");

            entity.HasOne(d => d.MaNdNavigation).WithMany(p => p.Danhgia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DANHGIA__MaND__6E01572D");
        });

        modelBuilder.Entity<Danhmuc>(entity =>
        {
            entity.HasKey(e => e.MaDm).HasName("PK__DANHMUC__2725866E4D853AF6");
        });

        modelBuilder.Entity<Diachi>(entity =>
        {
            entity.HasKey(e => e.MaDc).HasName("PK__DIACHI__27258664FB641100");

            entity.Property(e => e.MacDinh).HasDefaultValue(0);
            entity.Property(e => e.PhuongXa).HasDefaultValue("Đông Xuyên");
            entity.Property(e => e.QuanHuyen).HasDefaultValue("TP. Long Xuyên");
            entity.Property(e => e.TinhThanh).HasDefaultValue("An Giang");

            entity.HasOne(d => d.MaNdNavigation).WithMany(p => p.Diachis)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DIACHI__MaND__59FA5E80");
        });

        modelBuilder.Entity<Hoadon>(entity =>
        {
            entity.HasKey(e => e.MaHd).HasName("PK__HOADON__2725A6E09DEF826B");

            entity.Property(e => e.Ngay).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TongTien).HasDefaultValue(0);
            entity.Property(e => e.TrangThai).HasDefaultValue("Chờ xác nhận");

            entity.HasOne(d => d.MaNdNavigation).WithMany(p => p.Hoadons).HasConstraintName("FK__HOADON__MaND__628FA481");

            entity.HasOne(d => d.MaPtttNavigation).WithMany(p => p.Hoadons)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__HOADON__MaPTTT__1EA48E88");
        });

        modelBuilder.Entity<Khuyenmai>(entity =>
        {
            entity.HasKey(e => e.MaKm).HasName("PK__KHUYENMA__2725CF15D28B8A28");

            entity.Property(e => e.DieuKienApDung).HasDefaultValue(0);
            entity.Property(e => e.TrangThai).HasDefaultValue("HoatDong");
        });

        modelBuilder.Entity<Lichsumuahang>(entity =>
        {
            entity.HasKey(e => e.MaLs).HasName("PK__LICHSUMU__2725C772655EFE17");

            entity.Property(e => e.NgayMua).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TinhTrang).HasDefaultValue("Chờ xác nhận");
            entity.Property(e => e.TongTien).HasDefaultValue(0);

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.Lichsumuahangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LICHSUMUAH__MaHD__01142BA1");

            entity.HasOne(d => d.MaNdNavigation).WithMany(p => p.Lichsumuahangs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__LICHSUMUAH__MaND__00200768");

            entity.HasOne(d => d.MaPtttNavigation).WithMany(p => p.Lichsumuahangs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__LICHSUMUA__MaPTT__03F0984C");
        });

        modelBuilder.Entity<Mathang>(entity =>
        {
            entity.HasKey(e => e.MaMh).HasName("PK__MATHANG__2725DFD9846BE7BE");

            entity.Property(e => e.GiaBan).HasDefaultValue(0);
            entity.Property(e => e.GiaGoc).HasDefaultValue(0);
            entity.Property(e => e.LuotMua).HasDefaultValue(0);
            entity.Property(e => e.LuotXem).HasDefaultValue(0);
            entity.Property(e => e.SoLuong).HasDefaultValue(0);

            entity.HasOne(d => d.MaDmNavigation).WithMany(p => p.Mathangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MATHANG__MaDM__5165187F");

            entity.HasMany(d => d.MaKms).WithMany(p => p.MaMhs)
                .UsingEntity<Dictionary<string, object>>(
                    "MathangKhuyenmai",
                    r => r.HasOne<Khuyenmai>().WithMany()
                        .HasForeignKey("MaKm")
                        .HasConstraintName("FK_KHUYENMAI_KM"),
                    l => l.HasOne<Mathang>().WithMany()
                        .HasForeignKey("MaMh")
                        .HasConstraintName("FK_MATHANG_KM"),
                    j =>
                    {
                        j.HasKey("MaMh", "MaKm").HasName("PK_MATHANG_KM");
                        j.ToTable("MATHANG_KHUYENMAI");
                        j.IndexerProperty<int>("MaMh").HasColumnName("MaMH");
                        j.IndexerProperty<int>("MaKm").HasColumnName("MaKM");
                    });
        });

        modelBuilder.Entity<Nguoidung>(entity =>
        {
            entity.HasKey(e => e.MaNd).HasName("PK__NGUOIDUN__2725D72440A2D85E");

            entity.Property(e => e.LoaiNd).HasDefaultValue("Khách hàng");
        });

        modelBuilder.Entity<Phuongthucthanhtoan>(entity =>
        {
            entity.HasKey(e => e.MaPttt).HasName("PK__PHUONGTH__B30A2802968851D7");
        });

        modelBuilder.Entity<ThongkeDoanhthu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__THONGKE___3214EC272236025A");

            entity.HasOne(d => d.PhuongThucNavigation).WithMany(p => p.ThongkeDoanhthus).HasConstraintName("FK__THONGKE_D__Phuon__3E1D39E1");
        });

        modelBuilder.Entity<ThongkeMathangBan>(entity =>
        {
            entity.HasKey(e => e.MaTk).HasName("PK__THONGKE___272500709E68B9E7");

            entity.Property(e => e.ThoiGian).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaMhNavigation).WithMany(p => p.ThongkeMathangBans)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__THONGKE_MA__MaMH__08B54D69");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
