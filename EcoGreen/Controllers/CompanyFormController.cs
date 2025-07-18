using Application.Entities.Base;
using Application.Entities.Base.Post;
using Application.Interface.IServices;
using Application.Request.Activity;
using Application.Request.Post;
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
        private readonly ICommentService _commentService;
        public CompanyFormController(ICompanyFormService companyFormService, ICommentService commentService, IMapper mapper)
        {
            _companyFormService = companyFormService;
            _mapper = mapper;
            _commentService = commentService;
        }

        [HttpGet("get-all-activities")]
        public async Task<IActionResult> GetAllActivities()
        {
            var response = await _companyFormService.GetAllActivityForms();
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
    }
}
