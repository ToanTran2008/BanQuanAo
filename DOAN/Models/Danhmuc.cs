using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Models;

[Table("DANHMUC")]
[Index("Ten", Name = "UQ__DANHMUC__C451FA83BF71EE0B", IsUnique = true)]
public partial class Danhmuc
{
    [Key]
    [Column("MaDM")]
    public int MaDm { get; set; }

    [StringLength(100)]
    public string Ten { get; set; } = null!;

    [StringLength(255)]
    public string? MoTa { get; set; }

    [InverseProperty("MaDmNavigation")]
    public virtual ICollection<Mathang> Mathangs { get; set; } = new List<Mathang>();
}
