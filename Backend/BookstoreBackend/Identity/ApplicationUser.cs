using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace BookstoreBackend.Identity;

public class ApplicationUser : IdentityUser
{

    [Required]
    [MaxLength(50)]
    public required string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    public required string LastName { get; set; }

}