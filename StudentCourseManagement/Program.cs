using Microsoft.EntityFrameworkCore;
using Serilog;
using StudentCourseManagement.Configurations;
using StudentCourseManagement.Data;
using StudentCourseManagement.Extensions;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

// Services
builder.Services.AddControllers();
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureServices();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureValidation();
builder.Services.ConfigureMapping();
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("SMTP"));

var app = builder.Build();


// Middleware
app.UseAuthentication();
app.UseAuthorization();

app.ConfigureMiddleware();

// Auto-apply migrations when app starts
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.Migrate();
}

app.Run();