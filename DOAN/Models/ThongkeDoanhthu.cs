using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Models;

[Table("THONGKE_DOANHTHU")]
public partial class ThongkeDoanhthu
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    public int? Nam { get; set; }

    public int? Thang { get; set; }

    [Column(TypeName = "decimal(15, 2)")]
    public decimal? DoanhThu { get; set; }

    public int? PhuongThuc { get; set; }

    [ForeignKey("PhuongThuc")]
    [InverseProperty("ThongkeDoanhthus")]
    public virtual Phuongthucthanhtoan? PhuongThucNavigation { get; set; }
}
