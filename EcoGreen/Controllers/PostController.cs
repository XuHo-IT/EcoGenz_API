using Application.Entities.Base.Post;
using Application.Interface.IServices;
using Application.Request.Post;
using AutoMapper;
using EcoGreen.Service;
using Microsoft.AspNetCore.Mvc;

namespace EcoGreen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;
        private readonly ILikeService _likeService;

        private readonly IShareService _shareService;
        public PostController(IPostService postService, IMapper mapper, ILikeService likeService, IShareService shareService)
        {
            _postService = postService;
            _mapper = mapper;
            _likeService = likeService;

            _shareService = shareService;
        }

        [HttpGet("get-all-posts")]
        public async Task<IActionResult> GetAllPosts()
        {
            var response = await _postService.GetAllPosts();
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("search-posts")]
        public async Task<IActionResult> SearchPosts([FromQuery] PostSearchRequest request)
        {
            var response = await _postService.GetAllPostsWithSearchAndSort(request);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("get-posts/{postId}")]
        public async Task<IActionResult> GetPostById(Guid postId)
        {
            var response = await _postService.GetPostById(postId);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("create-post")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request, IFormFile? imageFile,
            [FromServices] CloudinaryService cloudinaryService)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var post = _mapper.Map<Post>(request);

            if (imageFile != null && imageFile.Length > 0)
            {
                var imageUrl = await cloudinaryService.UploadImageAsync(imageFile);
                post.MediaUrl = imageUrl;
            }
            else
            {
                post.MediaUrl = "/Helpers/profile_base.jpg";
            }
            var response = await _postService.CreatePost(post);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("update-post")]
        public async Task<IActionResult> UpdatePost([FromBody] UpdatePostRequest request)
        {
            var post = _mapper.Map<Post>(request);
            var response = await _postService.UpdatePost(post);
            return StatusCode((int)response.StatusCode, response);
        }


        [HttpDelete("delete-post/{postId}")]
        public async Task<IActionResult> Deletepost(Guid postId)
        {
            var response = await _postService.DeletePost(postId);
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpPost("like")]
        public async Task<IActionResult> ToggleLike([FromBody] LikeRequest request)
        {
            var response = await _likeService.ToggleLikeAsync(request.PostId, request.UserId);
            return StatusCode((int)response.StatusCode, response);
        }


        [HttpPost("share")]
        public async Task<IActionResult> SharePost([FromBody] ShareRequest request)
        {
            var share = _mapper.Map<Share>(request);

            var response = await _shareService.SharePostAsync(share);
            return StatusCode((int)response.StatusCode, response);
        }





    }
}
