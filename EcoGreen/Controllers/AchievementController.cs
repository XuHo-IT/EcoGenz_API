using Application.Request;
using InfrasStructure.EntityFramework.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoGreen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AchievementController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public AchievementController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet("get-all-achievements")]
        public async Task<IActionResult> GetAllAchievements()
        {
            var achievements = await _context.Achievements
                .Select(a => new AchievementRequest
                {
                    Id = a.Id,
                    Name = a.Name,
                    IconUrl = a.IconUrl,
                    AchievedAt = a.AchievedAt,
                    UserId = a.UserId,
                    Description = a.Description,

                }).ToListAsync();

            return Ok(achievements);
        }

        [HttpGet("get-by-user/{userId}")]
        public async Task<IActionResult> GetAchievementsByUserId(Guid userId)
        {
            var achievements = await _context.Achievements
                .Where(a => a.User.Id == userId) // or a.UserId == userId if you have UserId FK in Achievements
                .Select(a => new
                {
                    a.Id,
                    a.Name,
                    a.IconUrl,
                    User = new
                    {
                        a.User.Id,
                        a.User.UserName
                    }
                })
                .ToListAsync();

            if (achievements == null || achievements.Count == 0)
                return NotFound(new { message = "No achievements found for this user." });

            return Ok(achievements);
        }


    }
}
