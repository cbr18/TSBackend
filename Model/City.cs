using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSBackend.Model;

[Table("cities")]
public class City
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    public virtual List<Venue> Venues { get; set; } = [];
}

public class CityCreateDto
{
    public string Name { get; set; } = null!;
}