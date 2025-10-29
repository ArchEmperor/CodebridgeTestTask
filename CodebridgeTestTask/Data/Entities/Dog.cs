using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CodebridgeTestTask.Data.Entities;

[Table("dogs")]
public class Dog
{
    [Key]
    [Column("name")]
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public required string Name { get; set; }

    [Column("color")]
    [Required]
    public required string Color { get; set; }

    [Column("tail_length")]
    [Range(1, 1000, ErrorMessage = "Tail length must be between 1 and 1000.")]
    public required int TailLength { get; set; }

    [Column("weight")]
    [Range(1, 200, ErrorMessage = "Weight must be between 1 and 200.")]
    public required int Weight { get; set; }
}