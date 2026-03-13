using StudentCourseManagement.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureServices();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureValidation();
builder.Services.ConfigureMapping();    

var app = builder.Build();


builder.Services.ConfigureJwt(builder.Configuration);

app.UseAuthentication();
app.UseAuthorization();

app.ConfigureMiddleware();

app.Run();