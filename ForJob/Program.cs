using Microsoft.EntityFrameworkCore;
using ForJob.Services.EmailService;
using Hangfire;
using Hangfire.SQLite;
using ForJob.Services.TaskReminderService;
using Hangfire.Storage.SQLite;
using ForJob.Services.ServiceExtensions;
using ForJob.Controllers.Tasks.Post;
using FluentValidation;
using ForJob.Controllers.Tasks.Put;
using ForJob.Controllers.Users.Create;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var hangfireConnectionString = builder.Configuration.GetConnectionString("HangfireConnection");

builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddScoped<TaskReminderService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();

builder.Services.AddDbContext<ForJob.DbContext.DatabaseContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddValidatorsFromAssemblyContaining<TaskPostRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<TaskPutRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UserPostRequestValidator>();

builder.Services.AddHangfire(x =>
{
    x.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
    x.UseSimpleAssemblyNameTypeSerializer();
    x.UseRecommendedSerializerSettings();
    x.UseSQLiteStorage(hangfireConnectionString);
});

builder.Services.AddHangfireServer();
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UseHangfireDashboard();
app.MapHangfireDashboard("/hangfire");

app.AddHangfireJobs(builder.Configuration);

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
