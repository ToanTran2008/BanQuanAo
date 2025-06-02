using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Models;

[Table("KHUYENMAI")]
public partial class Khuyenmai
{
    [Key]
    [Column("MaKM")]
    public int MaKm { get; set; }

    [Column("TenKM")]
    [StringLength(255)]
    public string TenKm { get; set; } = null!;

    [StringLength(500)]
    public string? MoTa { get; set; }

    [StringLength(10)]
    public string LoaiGiamGia { get; set; } = null!;

    public int GiaTriGiam { get; set; }

    public int? DieuKienApDung { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayBatDau { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayKetThuc { get; set; }

    [StringLength(10)]
    public string? TrangThai { get; set; }

    [ForeignKey("MaKm")]
    [InverseProperty("MaKms")]
    public virtual ICollection<Mathang> MaMhs { get; set; } = new List<Mathang>();
}
