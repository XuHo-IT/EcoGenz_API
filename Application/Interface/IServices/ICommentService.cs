﻿using Application.Entities.Base.Post;
using Application.Response;

namespace Application.Interface.IServices
{
    public interface ICommentService
    {
        Task<APIResponse> AddCommentAsync(Comment comment);
    }
}
