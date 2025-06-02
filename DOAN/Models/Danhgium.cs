using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Models;

[Table("DANHGIA")]
public partial class Danhgium
{
    [Key]
    [Column("MaDG")]
    public int MaDg { get; set; }

    [Column("MaMH")]
    public int MaMh { get; set; }

    [Column("MaND")]
    public int MaNd { get; set; }

    public int? Rating { get; set; }

    [StringLength(500)]
    public string? BinhLuan { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDanhGia { get; set; }

    [ForeignKey("MaMh")]
    [InverseProperty("Danhgia")]
    public virtual Mathang MaMhNavigation { get; set; } = null!;

    [ForeignKey("MaNd")]
    [InverseProperty("Danhgia")]
    public virtual Nguoidung MaNdNavigation { get; set; } = null!;
}
