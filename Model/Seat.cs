using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TSBackend.Model;

[Table("seats")]
public class Seat
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("seat_number")]
    [Required]
    public int SeatNumber { get; set; }

    [ForeignKey(nameof(Row))]
    [Column("row_id")]
    [Required]
    public int RowId { get; set; }

    [JsonIgnore]
    public virtual Row Row { get; set; } = null!;

    [JsonIgnore]
    public virtual List<Ticket> Tickets { get; set; } = [];
}

public class SeatCreateDto
{
    [Required]
    public int SeatNumber { get; set; }

    [Required]
    public int RowId { get; set; }
}

public class SeatUpdateDto
{
    public int SeatNumber { get; set; }
}