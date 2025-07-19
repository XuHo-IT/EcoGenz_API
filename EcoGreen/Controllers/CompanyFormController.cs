using Application.Entities;
using Application.Entities.Base;
using Application.Entities.Base.Post;
using Application.Interface.IRepositories;
using Application.Interface.IServices;
using Application.Request.Activity;
using Application.Request.Post;
using AutoMapper;
using EcoGreen.Service;
using InfrasStructure.EntityFramework.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoGreen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyFormController : ControllerBase
    {
        private readonly ICompanyFormService _companyFormService;
        private readonly IMapper _mapper;
        private readonly ICommentService _commentService;
        private readonly IAuthRepository _authRepository;
        private readonly ApplicationDBContext _context;
        public CompanyFormController(ICompanyFormService companyFormService, ICommentService commentService, IMapper mapper, ApplicationDBContext context, IAuthRepository authRepository)
        {
            _companyFormService = companyFormService;
            _mapper = mapper;
            _commentService = commentService;
            _context = context;
            _authRepository = authRepository;
        }

        [HttpGet("get-all-activities")]
        public async Task<IActionResult> GetAllActivities()
        {
            var response = await _companyFormService.GetAllActivityForms();
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("get-all-activities-by-userid/{userId}")]
        public async Task<IActionResult> GetAllActivitiesByUserId(Guid userId)
        {
            var response = await _companyFormService.GetAllActivityFormsByUserId(userId);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("search-activities")]
        public async Task<IActionResult> SearchActivities([FromQuery] ActivitySearchRequest request)
        {
            var response = await _companyFormService.GetAllActivityFormsWithSearchAndSort(request);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("get-activity/{activityId}")]
        public async Task<IActionResult> GetActivityById(Guid activityId)
        {
            var response = await _companyFormService.GetActivityFormById(activityId);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("create-activity")]
        public async Task<IActionResult> CreateActivity([FromForm] CreateActivityRequest request, IFormFile? imageFile,
            [FromServices] CloudinaryService cloudinaryService)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var activity = _mapper.Map<Activity>(request);
            if (imageFile != null && imageFile.Length > 0)
            {
                var imageUrl = await cloudinaryService.UploadImageAsync(imageFile);
                activity.MediaUrl = imageUrl;
            }
            else
            {
                activity.MediaUrl = "/Helpers/profile_base.jpg";
            }
            var response = await _companyFormService.CreateActivityForm(activity);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("update-activity")]
        public async Task<IActionResult> UpdateActivity([FromBody] UpdateActivityRequest request)
        {
            var activity = _mapper.Map<Activity>(request);
            var response = await _companyFormService.UpdateActivityForm(activity);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("delete-activity/{activityId}")]
        public async Task<IActionResult> DeleteActivity(Guid activityId)
        {
            var response = await _companyFormService.DeleteActivityForm(activityId);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("comment")]
        public async Task<IActionResult> AddComment([FromBody] CommentRequest request)
        {
            var comment = _mapper.Map<Comment>(request);
            var response = await _commentService.AddCommentAsync(comment);
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpGet("list-comment/{activityId}")]
        public async Task<IActionResult> GetCommentByActivityId(Guid activityId)
        {
            var response = await _commentService.GetCommentByActivityId(activityId);
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpGet("list-comment")]
        public async Task<IActionResult> ListComment()
        {
            var response = await _commentService.ListCommentAsync();
            return StatusCode((int)response.StatusCode, response);
        }

        // POST: api/activities/register
        [HttpPost("register-activities")]
        public async Task<IActionResult> RegisterForActivity([FromBody] RegisterActivityRequest request)
        {
            var exists = await _context.Registrations
                .AnyAsync(ar => ar.UserId == request.UserId && ar.ActivityId == request.ActivityId);

            if (exists)
                return Ok(new { message = "User already registered." });

            var user = await _authRepository.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return NotFound(new { message = "User not found." });

            // Add the new registration
            var registration = new Registration
            {
                RegistrationId = Guid.NewGuid(),
                UserId = request.UserId,
                ActivityId = request.ActivityId,
                Status = RegistrationStatus.Pending
            };

            _context.Registrations.Add(registration);


            user.DoingAction += 1;
            user.ImpactPoints += 10;

            var achievementCandidates = new List<(int Threshold, string Name, string Description)>
{
    (10, "Seed Planter", "You're just getting started! Earned 10 impact points."),
    (25, "Helping Hand", "You're making a difference. Earned 25 points through kind acts."),
    (50, "Eco Hero", "You earned 50 impact points!"),
    (75, "Green Guardian", "You've taken the environment under your wing. 75 points strong!"),
    (100, "Charity Champion", "A true hero of charity. 100 points achieved!"),
    (150, "Earth Defender", "You've stood up for our planet with 150 points."),
    (200, "Hope Spreader", "Spreading hope through actions — 200 points milestone reached."),
    (300, "Impact Warrior", "You're a relentless force for good. 300 points earned!"),
    (500, "Legend of Good", "A legendary contributor. 500 points of pure impact!"),
    (750, "Planet Pioneer", "Blazing the trail for others. 750 points and counting."),
    (1000, "Global Guardian", "The ultimate protector of people and planet. 1000 points achieved!")
};


            foreach (var (threshold, name, description) in achievementCandidates.OrderByDescending(a => a.Threshold))
            {
                bool alreadyHas = user.Achievements.Any(a => a.Name == name);

                if (user.ImpactPoints >= threshold && !alreadyHas)
                {
                    var newAchievement = new Achievement
                    {
                        Id = Guid.NewGuid(),
                        Name = name,
                        Description = description,
                        UserId = user.Id
                    };

                    _context.Achievements.Add(newAchievement);
                    break;
                }
            }



            await _context.SaveChangesAsync();

            return Ok(new { message = "Registered successfully." });
        }


        // GET: api/activities/{activityId}/users
        [HttpGet("{activityId}/users")]
        public async Task<IActionResult> GetUsersForActivity(Guid activityId)
        {
            var users = await _context.Registrations
                .Where(ar => ar.ActivityId == activityId)
                .Select(ar => new
                {
                    ar.User.Id,
                    ar.User.UserName,
                    ar.User.Email
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/activities/{activityId}/registrations
        [HttpGet("{activityId}/registrations")]
        public async Task<IActionResult> GetRegistrationsForActivity(Guid activityId)
        {
            var registrations = await _context.Registrations
                .Where(r => r.ActivityId == activityId)
                .Include(r => r.User)
                .Select(r => new
                {
                    r.RegistrationId,
                    r.UserId,
                    r.Status,
                    r.Attended,
                    User = new
                    {
                        r.User.Id,
                        r.User.UserName,
                        r.User.Email,
                        r.User.ProfilePhotoUrl
                    }
                })
                .ToListAsync();

            return Ok(registrations);
        }

        // PUT: api/activities/registrations/{registrationId}/status
        [HttpPut("registrations/{registrationId}/status")]
        public async Task<IActionResult> UpdateRegistrationStatus(Guid registrationId, [FromBody] UpdateRegistrationStatusRequest request)
        {
            var registration = await _context.Registrations.FindAsync(registrationId);
            if (registration == null)
                return NotFound(new { message = "Registration not found." });

            registration.Status = request.Status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registration status updated successfully." });
        }

        // PUT: api/activities/registrations/{registrationId}/attendance
        [HttpPut("registrations/{registrationId}/attendance")]
        public async Task<IActionResult> UpdateRegistrationAttendance(Guid registrationId, [FromBody] UpdateAttendanceRequest request)
        {
            var registration = await _context.Registrations.FindAsync(registrationId);
            if (registration == null)
                return NotFound(new { message = "Registration not found." });

            registration.Attended = request.Attended;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Attendance updated successfully." });
        }
    }
}
