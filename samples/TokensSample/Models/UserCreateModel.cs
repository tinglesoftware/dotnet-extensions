using System.ComponentModel.DataAnnotations;

namespace TokensSample.Models;

public class UserCreateModel
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
}
