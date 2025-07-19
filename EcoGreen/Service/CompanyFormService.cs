using Application.Entities.Base;
using Application.Interface.IRepositories;
using Application.Interface.IServices;
using Application.Request.Activity;
using Application.Response;
using Common.Error;
using System.Linq.Expressions;
using System.Net;

namespace EcoGreen.Service
{
    public class CompanyFormService : ICompanyFormService
    {
        private readonly ICompanyFormRepository _companyFormRepository;

        public CompanyFormService(ICompanyFormRepository companyFormRepository)
        {
            _companyFormRepository = companyFormRepository;
        }

        public async Task<APIResponse> CreateActivityForm(Activity activityForm)
        {
            var response = new APIResponse();

            try
            {
                // Validate that the company user exists
                var companyUser = await _companyFormRepository.GetUserById(activityForm.CreatedByCompanyId);
                if (companyUser == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.isSuccess = false;
                    response.ErrorMessages.Add($"Company user with ID {activityForm.CreatedByCompanyId} not found.");
                    return response;
                }

                await _companyFormRepository.CreateActivityForm(activityForm);

                response.Result = "Form created successfully";
                response.StatusCode = HttpStatusCode.OK;
                response.isSuccess = true;
            }
            catch (Exception ex)
            {
                var error = new APIException((int)HttpStatusCode.BadRequest, ex.Message, ex.StackTrace);

                response.StatusCode = HttpStatusCode.BadRequest;
                response.isSuccess = false;
                response.ErrorMessages.Add(error.Message);
            }

            return response;
        }

        public async Task<APIResponse> DeleteActivityForm(Guid activityId)
        {
            var response = new APIResponse();
            try
            {
                await _companyFormRepository.DeleteActivityForm(activityId);
                response.Result = "Form deleted successfully";
                response.StatusCode = HttpStatusCode.OK;
                response.isSuccess = true;
            }
            catch (KeyNotFoundException ex)
            {
                var error = new APIException((int)HttpStatusCode.NotFound, ex.Message, ex.StackTrace);
                response.StatusCode = HttpStatusCode.NotFound;
                response.isSuccess = false;
                response.ErrorMessages.Add(error.Message);
            }
            catch (Exception ex)
            {
                var error = new APIException((int)HttpStatusCode.BadRequest, ex.Message, ex.StackTrace);

                response.StatusCode = HttpStatusCode.BadRequest;
                response.isSuccess = false;
                response.ErrorMessages.Add(error.Message);
            }

            return response;
        }

        public async Task<APIResponse> GetActivityFormBy(Expression<Func<Activity, bool>> predicate)
        {
            var response = new APIResponse();
            try
            {
                var result = await _companyFormRepository.GetActivityFormBy(predicate);
                if (result is null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.isSuccess = false;
                    response.ErrorMessages.Add("Not Found");
                }
                else
                {
                    response.Result = result;
                    response.StatusCode = HttpStatusCode.OK;
                    response.isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                var error = new APIException((int)HttpStatusCode.BadRequest, ex.Message, ex.StackTrace);

                response.StatusCode = HttpStatusCode.BadRequest;
                response.isSuccess = false;
                response.ErrorMessages.Add(error.Message);
            }
            return response;
        }

        public async Task<APIResponse> GetActivityFormById(Guid activityId)
        {
            var response = new APIResponse();
            try
            {
                var result = await _companyFormRepository.GetActivityFormById(activityId);
                if(result is null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.isSuccess = false;
                    response.ErrorMessages.Add("Not Found");
                } else
                {
                    response.Result = result;
                    response.StatusCode = HttpStatusCode.OK;
                    response.isSuccess = true;
                }
            }
            catch(Exception ex)
            {
                var error = new APIException((int)HttpStatusCode.BadRequest, ex.Message, ex.StackTrace);

                response.StatusCode = HttpStatusCode.BadRequest;
                response.isSuccess = false;
                response.ErrorMessages.Add(error.Message);
            }
            return response;
        }

        public async Task<APIResponse> GetAllActivityForms()
        {
            var response = new APIResponse();
            try
            {
                var result = await _companyFormRepository.GetAllActivityForms();
                response.Result = result;
                response.StatusCode = HttpStatusCode.OK;
                response.isSuccess = true;
            }
            catch(Exception ex)
            {
                var error = new APIException((int)HttpStatusCode.BadRequest, ex.Message, ex.StackTrace);

                response.StatusCode = HttpStatusCode.BadRequest;
                response.isSuccess = false;
                response.ErrorMessages.Add(error.Message);
            }
            return response;
        }

        public async Task<APIResponse> GetAllActivityFormsWithSearchAndSort(ActivitySearchRequest request)
        {
            var response = new APIResponse();
            try
            {
                // Validate sort field
                if (!string.IsNullOrWhiteSpace(request.SortBy) && !ActivitySortFields.IsValidSortField(request.SortBy))
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.isSuccess = false;
                    response.ErrorMessages.Add($"Invalid sort field '{request.SortBy}'. Valid fields are: {string.Join(", ", ActivitySortFields.ValidFields)}");
                    return response;
                }

                var result = await _companyFormRepository.GetAllActivityFormsWithSearchAndSort(request);
                response.Result = result;
                response.StatusCode = HttpStatusCode.OK;
                response.isSuccess = true;
            }
            catch(Exception ex)
            {
                var error = new APIException((int)HttpStatusCode.BadRequest, ex.Message, ex.StackTrace);

                response.StatusCode = HttpStatusCode.BadRequest;
                response.isSuccess = false;
                response.ErrorMessages.Add(error.Message);
            }
            return response;
        }

        public async Task<APIResponse> GetAllActivityFormsBy(Expression<Func<Activity, bool>> predicate)
        {
            var response = new APIResponse();
            try
            {
                var result = await _companyFormRepository.GetAllActivityFormsBy(predicate);
                response.Result = result;
                response.StatusCode = HttpStatusCode.OK;
                response.isSuccess = true;
            }
            catch(Exception ex)
            {
                var error = new APIException((int)HttpStatusCode.BadRequest, ex.Message, ex.StackTrace);

                response.StatusCode = HttpStatusCode.BadRequest;
                response.isSuccess = false;
                response.ErrorMessages.Add(error.Message);
            }
            return response;
        }

        public async Task<APIResponse> UpdateActivityForm(Activity activityForm)
        {
            var response = new APIResponse();
            try
            {
                await _companyFormRepository.UpdateActivityForm(activityForm);
                response.Result = "Form updated successfully.";
                response.StatusCode = HttpStatusCode.OK;
                response.isSuccess = true;
            }
            catch (KeyNotFoundException ex)
            {
                var error = new APIException((int)HttpStatusCode.NotFound, ex.Message, ex.StackTrace);
                response.StatusCode = HttpStatusCode.NotFound;
                response.isSuccess = false;
                response.ErrorMessages.Add(error.Message);
            }
            catch(Exception ex)
            {
                var error = new APIException((int)HttpStatusCode.BadRequest, ex.Message, ex.StackTrace);

                response.StatusCode = HttpStatusCode.BadRequest;
                response.isSuccess = false;
                response.ErrorMessages.Add(error.Message);
            }
            return response;
        }

        public async Task<APIResponse> GetAllActivityFormsForAIVoice()
        {
            var response = new APIResponse();
            try
            {
                var result = await _companyFormRepository.GetAllActivityFormsForAIVoice();
                response.Result = result;
                response.StatusCode = HttpStatusCode.OK;
                response.isSuccess = true;
            }
            catch (Exception ex)
            {
                var error = new APIException((int)HttpStatusCode.BadRequest, ex.Message, ex.StackTrace);

                response.StatusCode = HttpStatusCode.BadRequest;
                response.isSuccess = false;
                response.ErrorMessages.Add(error.Message);
            }
            return response;
        }
    }
}
