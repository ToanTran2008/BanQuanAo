using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Models;

[Table("NGUOIDUNG")]
public partial class Nguoidung
{
    [Key]
    [Column("MaND")]
    public int MaNd { get; set; }

    [StringLength(100)]
    public string Ten { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string? DienThoai { get; set; }

    [StringLength(50)]
    public string? Email { get; set; }

    [StringLength(255)]
    public string? MatKhau { get; set; }

    [Column("LoaiND")]
    [StringLength(50)]
    public string? LoaiNd { get; set; }

    [InverseProperty("MaNdNavigation")]
    public virtual ICollection<Danhgium> Danhgia { get; set; } = new List<Danhgium>();

    [InverseProperty("MaNdNavigation")]
    public virtual ICollection<Diachi> Diachis { get; set; } = new List<Diachi>();

    [InverseProperty("MaNdNavigation")]
    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();

    [InverseProperty("MaNdNavigation")]
    public virtual ICollection<Lichsumuahang> Lichsumuahangs { get; set; } = new List<Lichsumuahang>();
}
