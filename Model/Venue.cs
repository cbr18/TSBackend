using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TSBackend.Model;

[Table("venues")]
public class Venue
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("address")]
    [Required]
    public string Address { get; set; } = null!;

    [ForeignKey(nameof(City))]
    [Column("city_id")]
    [Required]
    public int CityId { get; set; }

    [JsonIgnore]
    public virtual City City { get; set; } = null!;

    [JsonIgnore]
    public virtual List<Hall> Halls { get; set; } = [];
}

public class VenueCreateDto
{
    [Required]
    public string Address { get; set; } = null!;

    [Required]
    public int CityId { get; set; }
}

public class VenueUpdateDto
{
    public string Address { get; set; } = null!;
}