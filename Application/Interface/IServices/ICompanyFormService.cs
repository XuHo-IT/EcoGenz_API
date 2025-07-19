using Application.Entities.Base;
using Application.Entities.DTOs;
using Application.Request.Activity;
using Application.Response;
using System.Linq.Expressions;

namespace Application.Interface.IServices
{
    public interface ICompanyFormService
    {
        Task<APIResponse> GetActivityFormById(Guid activityId);
        Task<APIResponse> GetAllActivityForms();
        Task<APIResponse> GetAllActivityFormsWithSearchAndSort(ActivitySearchRequest request);
        Task<APIResponse> GetAllActivityFormsBy(Expression<Func<Activity, bool>> predicate);
        Task<APIResponse> GetAllActivityFormsByUserId(Guid userId);
        Task<APIResponse> GetActivityFormBy(Expression<Func<Activity, bool>> predicate);
        Task<APIResponse> CreateActivityForm(Activity activityForm);
        Task<APIResponse> UpdateActivityForm(Activity activityForm);
        Task<APIResponse> DeleteActivityForm(Guid activityId);
        Task<APIResponse> GetAllActivityFormsForAIVoice();
    }
}
