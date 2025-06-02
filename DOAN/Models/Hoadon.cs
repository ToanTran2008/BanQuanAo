using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Models;

[Table("HOADON")]
public partial class Hoadon
{
    [Key]
    [Column("MaHD")]
    public int MaHd { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Ngay { get; set; }

    public int? TongTien { get; set; }

    [Column("MaND")]
    public int MaNd { get; set; }

    [StringLength(50)]
    public string TrangThai { get; set; } = null!;

    [Column("MaPTTT")]
    public int? MaPttt { get; set; }

    [InverseProperty("MaHdNavigation")]
    public virtual ICollection<Cthoadon> Cthoadons { get; set; } = new List<Cthoadon>();

    [InverseProperty("MaHdNavigation")]
    public virtual ICollection<Lichsumuahang> Lichsumuahangs { get; set; } = new List<Lichsumuahang>();

    [ForeignKey("MaNd")]
    [InverseProperty("Hoadons")]
    public virtual Nguoidung MaNdNavigation { get; set; } = null!;

    [ForeignKey("MaPttt")]
    [InverseProperty("Hoadons")]
    public virtual Phuongthucthanhtoan? MaPtttNavigation { get; set; }
}
