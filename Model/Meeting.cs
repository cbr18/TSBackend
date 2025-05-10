using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TSBackend.Model;

[Table("meetings")]
public class Meeting
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [Required]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("date")]
    [Required]
    public DateTime Date { get; set; }

    [Column("has_seat_map")]
    public bool HasSeatMap { get; set; } = false;

    [ForeignKey(nameof(Hall))]
    [Column("hall_id")]
    [Required]
    public int HallId { get; set; }

    [JsonIgnore]
    public virtual Hall Hall { get; set; } = null!;

    [JsonIgnore]
    public virtual List<Ticket> Tickets { get; set; } = [];

    public int GetTotalSeatsCount()
    {
        return Hall.GetTotalSeats();
    }

    public int GetSoldSeatsCount()
    {
        return Tickets.Count;
    }

    public int GetAvailableSeatsCount()
    {
        return GetTotalSeatsCount() - GetSoldSeatsCount();
    }
}

public class MeetingCreateDto
{
    [Required]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public int HallId { get; set; }

    public bool HasSeatMap { get; set; } = false;
}

public class MeetingUpdateDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public bool HasSeatMap { get; set; }
}