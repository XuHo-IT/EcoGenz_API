﻿using Application.Interface;
using Application.Interface.IServices;
using EcoGreen.Helpers;
using EcoGreen.Service;
using EcoGreen.Service.Chat;

namespace EcoGreen.Extensions
{
    public static class ServiceCfgExtension
    {
        public static IServiceCollection AddService(this IServiceCollection services)
        {
            services.AddScoped<ICompanyFormService, CompanyFormService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<CloudinaryService>();
            services.AddScoped<VisionService>();
            services.AddScoped<AIChatService>();

            return services;
        }
    }
}
