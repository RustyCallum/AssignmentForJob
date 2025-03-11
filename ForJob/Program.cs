using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ForJob.Services.EmailService;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Hangfire;
using Hangfire.SQLite;
using ForJob.Services.TaskReminderService;
using Hangfire.SqlServer;
using Hangfire.Storage.SQLite;
using SQLite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var hangifreConnectionString = builder.Configuration.GetConnectionString("HangfireConnection");

builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddScoped<TaskReminderService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Konfiguracja JWT Bearer w Swaggerze
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

builder.Services.AddDbContext<ForJob.DbContext.DatabaseContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddHangfire(x =>
{
    x.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
    x.UseSimpleAssemblyNameTypeSerializer();
    x.UseRecommendedSerializerSettings();
    x.UseSQLiteStorage(hangifreConnectionString);
});

builder.Services.AddHangfireServer();

builder.Services.AddAuthentication(y =>
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Aleksander51HaHaXDbekazwasxDxDnienawidzewas"))

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

var app = builder.Build();

app.UseHangfireDashboard();
app.MapHangfireDashboard("/hangfire");

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var taskReminderService = serviceProvider.GetRequiredService<TaskReminderService>();

    RecurringJob.AddOrUpdate(
        "task-reminder-job",
        () => taskReminderService.CheckTasksAndSendReminders(),  // Poprawne wywołanie metody
        "* * * * *"
    );
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
