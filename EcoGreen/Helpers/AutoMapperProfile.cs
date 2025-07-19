using Application.Entities.Base;
using Application.Entities.Base.Post;
using Application.Entities.DTOs.User;
using Application.Request.Activity;
using Application.Request.Post;
using Application.Request.User;
using AutoMapper;

namespace EcoGreen.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User registration DTO to User entity
            CreateMap<UserRegisterDTO, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.ProfilePhotoUrl, opt => opt.Ignore());

            // CreatePostRequest to Post
            CreateMap<CreatePostRequest, Post>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.MediaUrl, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.User, opt => opt.Ignore());
            //.ForMember(dest => dest.Likes, opt => opt.Ignore())
            //.ForMember(dest => dest.Shares, opt => opt.Ignore())
            //.ForMember(dest => dest.Comments, opt => opt.Ignore());

            // UpdatePostRequest to Post
            CreateMap<UpdatePostRequest, Post>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.MediaUrl, opt => opt.MapFrom(src => src.MediaUrl))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.User, opt => opt.Ignore());
            //.ForMember(dest => dest.Comments, opt => opt.Ignore())
            //.ForMember(dest => dest.Likes, opt => opt.Ignore())
            //.ForMember(dest => dest.Shares, opt => opt.Ignore());

            // CommentRequest to Comment
            CreateMap<CommentRequest, Comment>()
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Activity, opt => opt.Ignore());

            // ShareRequest to Share
            CreateMap<ShareRequest, Share>()
                .ForMember(dest => dest.SharedAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Post, opt => opt.Ignore());

            //CreateActivity
            CreateMap<CreateActivityRequest, Activity>()
    .ForMember(dest => dest.ActivityId, opt => opt.MapFrom(src => Guid.NewGuid()))
    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
    .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
    .ForMember(dest => dest.MediaUrl, opt => opt.Ignore())
    .ForMember(dest => dest.AmountOfPeople, opt => opt.MapFrom(src => src.AmountOfPeople))
    .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
    .ForMember(dest => dest.CreatedByCompanyId, opt => opt.MapFrom(src => src.CreatedByCompanyId))
    .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(src => src.IsApproved))
            .ForMember(dest => dest.Comments, opt => opt.Ignore());

            //UpdateActivity
            CreateMap<UpdateActivityRequest, Activity>()
     .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
     .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
     .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
     .ForMember(dest => dest.AmountOfPeople, opt => opt.MapFrom(src => src.AmountOfPeople))
     .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
     .ForMember(dest => dest.ActivityId, opt => opt.Ignore())
     .ForMember(dest => dest.CreatedByCompanyId, opt => opt.Ignore())
     .ForMember(dest => dest.IsApproved, opt => opt.Ignore());

            CreateMap<UserUpdateRequest, User>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserEmail))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.ProfilePhotoUrl, opt => opt.Ignore());


        }
    }
}
