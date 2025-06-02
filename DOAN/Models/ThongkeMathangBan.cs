using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Models;

[Table("THONGKE_MATHANG_BAN")]
public partial class ThongkeMathangBan
{
    [Key]
    [Column("MaTK")]
    public int MaTk { get; set; }

    [Column("MaMH")]
    public int MaMh { get; set; }

    public int SoLuongBan { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ThoiGian { get; set; }

    [ForeignKey("MaMh")]
    [InverseProperty("ThongkeMathangBans")]
    public virtual Mathang MaMhNavigation { get; set; } = null!;
}
