using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalApi.Domain.Entities;

public class Vehicle
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public required string Name { get; set; }

    [Required]
    [StringLength(100)]
    public required string Make { get; set; }

    [Required]
    public int Year { get; set; }
}