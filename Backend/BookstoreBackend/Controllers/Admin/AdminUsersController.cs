using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookstoreBackend.Identity;
using BookstoreBackend.DTOs.Admin;

namespace BookstoreBackend.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("api/Admin/users")]
    [ApiController]
    public class AdminUsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminUsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDTO>();

            foreach (var user in users)
            {
                userDtos.Add(new UserDTO
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Roles = await _userManager.GetRolesAsync(user)
                });
            }
            return Ok(userDtos);
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] UpdateUserRoleDTO dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null) return NotFound("User not found.");

            var roleExists = await _roleManager.RoleExistsAsync(dto.RoleName);
            if (!roleExists) return BadRequest("Role does not exist.");

            var result = await _userManager.AddToRoleAsync(user, dto.RoleName);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(new { message = $"Role '{dto.RoleName}' assigned successfully." });
        }

        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveRole([FromBody] UpdateUserRoleDTO dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null) return NotFound("User not found.");

            var result = await _userManager.RemoveFromRoleAsync(user, dto.RoleName);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(new { message = $"Role '{dto.RoleName}' removed successfully." });
        }
    }
}