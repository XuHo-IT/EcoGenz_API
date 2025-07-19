using Application.Entities.Base;
using Application.Entities.DTOs;
using Application.Request.Activity;
using System.Linq.Expressions;

namespace Application.Interface.IRepositories
{
    public interface ICompanyFormRepository
    {
        Task<IEnumerable<Activity>> GetAllActivityForms();
        Task<PagedResult<Activity>> GetAllActivityFormsWithSearchAndSort(ActivitySearchRequest request);
        Task<IEnumerable<Activity>> GetAllActivityFormsBy(Expression<Func<Activity, bool>> predicate);
        Task<IEnumerable<Activity>> GetAllActivityFormsByUserId(Guid userId);
        Task<Activity> GetActivityFormById(Guid activityId);
        Task<Activity> GetActivityFormBy(Expression<Func<Activity, bool>> predicate);
        Task CreateActivityForm(Activity activityForm);
        Task UpdateActivityForm(Activity activityForm);
        Task DeleteActivityForm(Guid activityId);
        Task<User> GetUserById(Guid userId);
        Task<IEnumerable<ActivityDTO>> GetAllActivityFormsForAIVoice();
    }
}
