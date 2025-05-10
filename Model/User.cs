using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TSBackend.Model;
[Table("users")]
public class User(string Name, string Email, string PasswordHash) : IdentityUser<int>
{
    public User() : this(string.Empty, string.Empty, string.Empty) { }

    [Column("id")]
    [Key]
    override public int Id { get; set; }

    [Column("name")]
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = Name;

    [Required]
    [Column("email")]
    override public string? Email { get; set; } = Email;

    [Required]
    [Column("passwordhash")]
    override public string? PasswordHash { get; set; } = PasswordHash;

    public virtual List<Ticket> Tickets { get; set; } = [];
}

public class UserCreateDto
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}

public class UserUpdateDto
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
}

public class UserLoginDto
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
