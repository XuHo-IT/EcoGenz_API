using Application.Entities;
using Application.Entities.Base;
using Application.Entities.Base.Post;
using Application.Entities.DTOs;
using Application.Interface;
using Application.Interface.IRepositories;
using Application.Request.Post;
using InfrasStructure.EntityFramework.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InfrasStructure.EntityFramework.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IAuthRepository _authRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PostRepository(ApplicationDBContext context, IUnitOfWork unitOfWork, IAuthRepository authRepository)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _authRepository = authRepository;
        }

        public async Task CreatePost(Post Post)
        {
            var user = await _authRepository.FindByIdAsync(Post.UserId.ToString());
            if (user == null)
                throw new Exception("User not found.");
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
            Post.CreatedAt = DateTime.UtcNow;
            await _context.Posts.AddAsync(Post);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeletePost(Guid postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<IEnumerable<Post>> GetAllPost()
        {
            return await _context.Posts
                                   .Include(a => a.User)
                                   .AsNoTracking()
                                   .ToListAsync();

        }

        public async Task<IEnumerable<Post>> GetAllPostBy(Expression<Func<Post, bool>> predicate)
        {
            return await _context.Posts
                               .Where(predicate)
                               .Include(a => a.User)
                               .AsNoTracking()
                               .ToListAsync();
        }

        public async Task<PagedResult<Post>> GetAllPostWithSearchAndSort(PostSearchRequest request)
        {
            var query = _context.Posts.Include(a => a.User).AsQueryable();

            // Apply search filters
            query = ApplySearchFilters(query, request);

            // Get total count before pagination
            var totalRecords = await query.CountAsync();

            // Apply sorting
            query = ApplySorting(query, request.SortBy, request.SortDirection);

            // Apply pagination
            var posts = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return PagedResult<Post>.Create(
                posts,
                totalRecords,
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.SortBy,
                request.SortDirection.ToString()
            );
        }

        public async Task<Post?> GetPostBy(Expression<Func<Post, bool>> predicate)
        {
            return await _context.Posts
                           .Include(a => a.User)
                           .FirstOrDefaultAsync(predicate);
        }

        public async Task<Post?> GetPostById(Guid PostId)
        {
            return await _context.Posts
                                .Include(a => a.User)
                                .FirstOrDefaultAsync(a => a.Id == PostId);
        }

        public async Task<User> GetUserById(Guid userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task UpdatePost(Post Post)
        {
            var existingPost = await _context.Posts.FindAsync(Post.Id);

            if (existingPost == null)
            {
                throw new KeyNotFoundException($"Post with ID {Post.Id} not found.");
            }

            _context.Entry(existingPost).CurrentValues.SetValues(Post);

            existingPost.CreatedAt = DateTime.SpecifyKind(existingPost.CreatedAt, DateTimeKind.Utc);

            _context.Entry(existingPost).Property(p => p.UserId).IsModified = false;

            await _unitOfWork.SaveChangesAsync();
        }
        private IQueryable<Post> ApplySearchFilters(IQueryable<Post> query, PostSearchRequest request)
        {
            // Global search term (searches in title, description, location)
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(a =>
                    a.Content.ToLower().Contains(searchTerm) ||
                    a.User.UserName.ToLower().Contains(searchTerm)
                );
            }

            // Specific field filters
            if (!string.IsNullOrWhiteSpace(request.Content))
            {
                query = query.Where(a => a.Content.ToLower().Contains(request.Content.ToLower()));
            }
            // Date range filters
            if (request.DateFrom.HasValue)
            {
                query = query.Where(a => a.CreatedAt >= request.DateFrom.Value);
            }

            if (request.DateTo.HasValue)
            {
                query = query.Where(a => a.CreatedAt <= request.DateTo.Value);
            }

            return query;
        }

        private IQueryable<Post> ApplySorting(IQueryable<Post> query, string? sortBy, Application.Request.SortDirection sortDirection)
        {
            if (string.IsNullOrWhiteSpace(sortBy) || !PostSortFields.IsValidSortField(sortBy))
            {
                sortBy = PostSortFields.CreatedAt; // Default sort
            }

            var isDescending = sortDirection == Application.Request.SortDirection.Desc;

            return sortBy.ToLower() switch
            {
                "content" => isDescending ? query.OrderByDescending(a => a.Content) : query.OrderBy(a => a.Content),
                "createdat" => isDescending ? query.OrderByDescending(a => a.CreatedAt) : query.OrderBy(a => a.CreatedAt),
                "createdbyuserid" => isDescending ? query.OrderByDescending(a => a.UserId) : query.OrderBy(a => a.UserId),
                _ => isDescending ? query.OrderByDescending(a => a.CreatedAt) : query.OrderBy(a => a.CreatedAt)
            };
        }
    }
}
