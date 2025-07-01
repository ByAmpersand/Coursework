using System.ComponentModel.DataAnnotations;

namespace BookstoreBackend.DTOs.Admin
{
    public class UpdateUserRoleDTO
    {
        [Required]
        public required string UserId { get; set; }
        [Required]
        public required string RoleName { get; set; }
    }
}