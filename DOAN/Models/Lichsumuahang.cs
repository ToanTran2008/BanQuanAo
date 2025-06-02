using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Models;

[Table("LICHSUMUAHANG")]
public partial class Lichsumuahang
{
    [Key]
    [Column("MaLS")]

   // [DisplayName ="Mã Lịch sử"]
    public int MaLs { get; set; }


    [Column("MaND")]
    public int? MaNd { get; set; }

    [Column("MaHD")]
    public int MaHd { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayMua { get; set; }

    public int? TongTien { get; set; }

    [Column("MaPTTT")]
    public int? MaPttt { get; set; }

    [StringLength(50)]
    public string TinhTrang { get; set; } = null!;

    [StringLength(255)]
    public string? GhiChu { get; set; }

    [ForeignKey("MaHd")]
    [InverseProperty("Lichsumuahangs")]
    public virtual Hoadon MaHdNavigation { get; set; } = null!;

    [ForeignKey("MaNd")]
    [InverseProperty("Lichsumuahangs")]
    public virtual Nguoidung? MaNdNavigation { get; set; }

    [ForeignKey("MaPttt")]
    [InverseProperty("Lichsumuahangs")]
    public virtual Phuongthucthanhtoan? MaPtttNavigation { get; set; }
}
