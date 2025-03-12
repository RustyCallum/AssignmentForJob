using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using ForJob.Services.TaskReminderService;
using System.Configuration;

namespace ForJob.Services.ServiceExtensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
            });

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(y =>
            {
                y.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                y.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                y.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.Authority = "http://localhost:7094/";
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = false,
                    ValidAudience = "http://localhost:7094/",
                    ValidIssuer = "http://localhost:7094/",
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        "Aleksander51HaHaXDbekazwasxDxDnienawidzewas"
                    ))
                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = y =>
                    {
                        y.Token = y.Request.Cookies["token"];
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }

        public static IApplicationBuilder AddHangfireJobs(this IApplicationBuilder app, IConfiguration configuration)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var taskReminderService = serviceProvider.GetRequiredService<TaskReminderService.TaskReminderService>();

                var cronExpression = configuration.GetValue<string>("Hangfire:TaskReminderCron") ?? "*/5 * * * *";

                RecurringJob.AddOrUpdate(
                    "task-reminder-job",
                    () => taskReminderService.CheckTasksAndSendReminders(),
                    cronExpression
                );
            }

            return app;
        }
    }
}
