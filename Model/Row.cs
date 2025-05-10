using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TSBackend.Model;

[Table("rows")]
public class Row
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("row_number")]
    [Required]
    public int RowNumber { get; set; }

    [Column("seats_count")]
    [Required]
    public int SeatsCount { get; set; }

    [ForeignKey(nameof(Hall))]
    [Column("hall_id")]
    [Required]
    public int HallId { get; set; }

    [JsonIgnore]
    public virtual Hall Hall { get; set; } = null!;

    [JsonIgnore]
    public virtual List<Seat> Seats { get; set; } = [];
}

public class RowCreateDto
{
    [Required]
    public int RowNumber { get; set; }

    [Required]
    public int SeatsCount { get; set; }

    [Required]
    public int HallId { get; set; }
}

public class RowUpdateDto
{
    public int Id { get; set; }
    public int RowNumber { get; set; }
    public int SeatsCount { get; set; }
}