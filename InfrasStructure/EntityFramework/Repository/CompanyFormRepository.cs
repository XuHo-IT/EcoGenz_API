using Application.Entities.Base;
using Application.Entities.DTOs;
using Application.Interface;
using Application.Interface.IRepositories;
using Application.Request;
using Application.Request.Activity;
using InfrasStructure.EntityFramework.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InfrasStructure.EntityFramework.Repository
{
    public class CompanyFormRepository : ICompanyFormRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public CompanyFormRepository(ApplicationDBContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<Activity?> GetActivityFormById(Guid activityId)
        {
            return await _context.Activities
                                 .Include(a => a.CompanyUser)
                                 .FirstOrDefaultAsync(a => a.ActivityId == activityId);
        }

        public async Task<IEnumerable<Activity>> GetAllActivityForms()
        {
            return await _context.Activities
                                 .Include(a => a.CompanyUser)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Activity>> GetAllActivityFormsBy(Expression<Func<Activity, bool>> predicate)
        {
            return await _context.Activities
                                 .Where(predicate)
                                 .Include(a => a.CompanyUser)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<Activity?> GetActivityFormBy(Expression<Func<Activity, bool>> predicate)
        {
            return await _context.Activities
                                 .Include(a => a.CompanyUser)
                                 .FirstOrDefaultAsync(predicate);
        }

        public async Task CreateActivityForm(Activity activityForm)
        {
            activityForm.Date = DateTime.SpecifyKind(activityForm.Date, DateTimeKind.Utc);
            await _context.Activities.AddAsync(activityForm);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateActivityForm(Activity activityForm)
        {
            var existingActivity = await _context.Activities.FindAsync(activityForm.ActivityId);

            if (existingActivity == null)
            {
                throw new KeyNotFoundException($"Activity with ID {activityForm.ActivityId} not found.");
            }

            _context.Entry(existingActivity).CurrentValues.SetValues(activityForm);

            existingActivity.Date = DateTime.SpecifyKind(activityForm.Date, DateTimeKind.Utc);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteActivityForm(Guid activityId)
        {
            var activityToDelete = await _context.Activities.FindAsync(activityId);

            if (activityToDelete == null)
            {
                throw new KeyNotFoundException($"Activity with ID {activityId} not found for deletion.");
            }

            _context.Activities.Remove(activityToDelete);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<User> GetUserById(Guid userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<PagedResult<Activity>> GetAllActivityFormsWithSearchAndSort(ActivitySearchRequest request)
        {
            var query = _context.Activities.Include(a => a.CompanyUser).AsQueryable();

            // Apply search filters
            query = ApplySearchFilters(query, request);

            // Get total count before pagination
            var totalRecords = await query.CountAsync();

            // Apply sorting
            query = ApplySorting(query, request.SortBy, request.SortDirection);

            // Apply pagination
            var activities = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return PagedResult<Activity>.Create(
                activities,
                totalRecords,
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.SortBy,
                request.SortDirection.ToString()
            );
        }

        private IQueryable<Activity> ApplySearchFilters(IQueryable<Activity> query, ActivitySearchRequest request)
        {
            // Global search term (searches in title, description, location)
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(a =>
                    a.Title.ToLower().Contains(searchTerm) ||
                    a.Description.ToLower().Contains(searchTerm) ||
                    a.Location.ToLower().Contains(searchTerm) ||
                    a.CompanyUser.UserName.ToLower().Contains(searchTerm)
                );
            }

            // Specific field filters
            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                query = query.Where(a => a.Title.ToLower().Contains(request.Title.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                query = query.Where(a => a.Description.ToLower().Contains(request.Description.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(request.Location))
            {
                query = query.Where(a => a.Location.ToLower().Contains(request.Location.ToLower()));
            }

            if (request.CreatedByCompanyId.HasValue)
            {
                query = query.Where(a => a.CreatedByCompanyId == request.CreatedByCompanyId);
            }

            if (request.IsApproved.HasValue)
            {
                query = query.Where(a => a.IsApproved == request.IsApproved);
            }

            // Date range filters
            if (request.DateFrom.HasValue)
            {
                query = query.Where(a => a.Date >= request.DateFrom.Value);
            }

            if (request.DateTo.HasValue)
            {
                query = query.Where(a => a.Date <= request.DateTo.Value);
            }

            // Amount of people filters
            if (request.MinPeople.HasValue)
            {
                query = query.Where(a => a.AmountOfPeople >= request.MinPeople);
            }

            if (request.MaxPeople.HasValue)
            {
                query = query.Where(a => a.AmountOfPeople <= request.MaxPeople);
            }

            return query;
        }

        private IQueryable<Activity> ApplySorting(IQueryable<Activity> query, string? sortBy, SortDirection sortDirection)
        {
            if (string.IsNullOrWhiteSpace(sortBy) || !ActivitySortFields.IsValidSortField(sortBy))
            {
                sortBy = ActivitySortFields.Date; // Default sort
            }

            var isDescending = sortDirection == SortDirection.Desc;

            return sortBy.ToLower() switch
            {
                "title" => isDescending ? query.OrderByDescending(a => a.Title) : query.OrderBy(a => a.Title),
                "description" => isDescending ? query.OrderByDescending(a => a.Description) : query.OrderBy(a => a.Description),
                "location" => isDescending ? query.OrderByDescending(a => a.Location) : query.OrderBy(a => a.Location),
                "date" => isDescending ? query.OrderByDescending(a => a.Date) : query.OrderBy(a => a.Date),
                "amountofpeople" => isDescending ? query.OrderByDescending(a => a.AmountOfPeople) : query.OrderBy(a => a.AmountOfPeople),
                "isapproved" => isDescending ? query.OrderByDescending(a => a.IsApproved) : query.OrderBy(a => a.IsApproved),
                "createdbycompanyid" => isDescending ? query.OrderByDescending(a => a.CreatedByCompanyId) : query.OrderBy(a => a.CreatedByCompanyId),
                _ => isDescending ? query.OrderByDescending(a => a.Date) : query.OrderBy(a => a.Date)
            };
        }

        public async Task<IEnumerable<ActivityDTO>> GetAllActivityFormsForAIVoice()
        {
            return await _context.Activities
                                 .Select(a => new ActivityDTO
                                 {
                                     Title = a.Title,
                                     Description = a.Description,
                                     Location = a.Location,
                                     Date = a.Date,
                                 }).ToListAsync();
        }
    }
}