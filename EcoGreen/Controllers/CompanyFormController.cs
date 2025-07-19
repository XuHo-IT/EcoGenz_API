using Application.Entities.Base;
using Application.Interface.IServices;
using Application.Request.Activity;
using AutoMapper;
using EcoGreen.Service;
using Microsoft.AspNetCore.Mvc;

namespace EcoGreen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyFormController : ControllerBase
    {
        private readonly ICompanyFormService _companyFormService;
        private readonly IMapper _mapper;
        public CompanyFormController(ICompanyFormService companyFormService, IMapper mapper)
        {
            _companyFormService = companyFormService;
            _mapper = mapper;
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
    }
}
