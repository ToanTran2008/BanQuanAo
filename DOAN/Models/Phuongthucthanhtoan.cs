using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Models;

[Table("PHUONGTHUCTHANHTOAN")]
[Index("TenPttt", Name = "UQ__PHUONGTH__FD193B4F806FB889", IsUnique = true)]
public partial class Phuongthucthanhtoan
{
    [Key]
    [Column("MaPTTT")]
    public int MaPttt { get; set; }

    [Column("TenPTTT")]
    [StringLength(50)]
    public string TenPttt { get; set; } = null!;

    [InverseProperty("MaPtttNavigation")]
    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();

    [InverseProperty("MaPtttNavigation")]
    public virtual ICollection<Lichsumuahang> Lichsumuahangs { get; set; } = new List<Lichsumuahang>();

    [InverseProperty("PhuongThucNavigation")]
    public virtual ICollection<ThongkeDoanhthu> ThongkeDoanhthus { get; set; } = new List<ThongkeDoanhthu>();
}
