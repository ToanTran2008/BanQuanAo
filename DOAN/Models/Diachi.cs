using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Models;

[Table("DIACHI")]
public partial class Diachi
{
    [Key]
    [Column("MaDC")]
    public int MaDc { get; set; }

    [Column("MaND")]
    public int MaNd { get; set; }

    [StringLength(100)]
    public string ChiTiet { get; set; } = null!;

    [StringLength(50)]
    public string? PhuongXa { get; set; }

    [StringLength(50)]
    public string? QuanHuyen { get; set; }

    [StringLength(50)]
    public string? TinhThanh { get; set; }

    public int? MacDinh { get; set; }

    [ForeignKey("MaNd")]
    [InverseProperty("Diachis")]
    public virtual Nguoidung MaNdNavigation { get; set; } = null!;
}
