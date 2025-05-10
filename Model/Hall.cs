using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TSBackend.Model;

[Table("halls")]
public class Hall
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [Required]
    public string Name { get; set; } = null!;

    [Column("capacity")]
    [Required]
    public int Capacity { get; set; }

    [ForeignKey(nameof(Venue))]
    [Column("venue_id")]
    [Required]
    public int VenueId { get; set; }

    [JsonIgnore]
    public virtual Venue Venue { get; set; } = null!;

    [JsonIgnore]
    public virtual List<Row> Rows { get; set; } = [];

    [JsonIgnore]
    public virtual List<Meeting> Meetings { get; set; } = [];

    public int GetTotalSeats()
    {
        return Rows.Sum(r => r.SeatsCount);
    }
}

public class HallCreateDto
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public int Capacity { get; set; }

    [Required]
    public int VenueId { get; set; }
}

public class HallUpdateDto
{
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
}