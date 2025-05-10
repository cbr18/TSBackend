using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TSBackend.Model;

[Table("tickets")]
public class Ticket
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [ForeignKey(nameof(Meeting))]
    [Column("meeting_id")]
    [Required]
    public int MeetingId { get; set; }

    [JsonIgnore]
    public virtual Meeting Meeting { get; set; } = null!;

    [ForeignKey(nameof(User))]
    [Column("user_id")]
    [Required]
    public int UserId { get; set; }

    [JsonIgnore]
    public virtual User User { get; set; } = null!;

    [ForeignKey(nameof(Seat))]
    [Column("seat_id")]
    [Required]
    public int? SeatId { get; set; }

    [JsonIgnore]
    public virtual Seat Seat { get; set; } = null!;

    [Column("purchase_date")]
    [Required]
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
}

public class TicketCreateDto
{
    [Required]
    public int MeetingId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int? SeatId { get; set; }
}

public class TicketDetailsDto
{
    public int Id { get; set; }
    public string MeetingName { get; set; } = null!;
    public int MeetingId { get; set; }
    public int UserId { get; set; }
    public DateTime MeetingDate { get; set; }
    public string VenueName { get; set; } = null!;
    public string? HallName { get; set; } = null!;
    public int? RowNumber { get; set; }
    public int? SeatNumber { get; set; }
    public string UserName { get; set; } = null!;
    public DateTime PurchaseDate { get; set; }
}