using LucaHome.Models;
using LucaHome.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
            build =>
            {
                build.WithOrigins("https://lucapanariello.altervista.org", "http://lucapanariello.altervista.org");                
                build.AllowAnyMethod();
                build.AllowAnyHeader();                          
            });    
});

// Add services to the container.
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("CommentDatabase"));

builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<CommentService>();
builder.Services.AddSingleton<ProjectService>();
builder.Services.AddSingleton<SkillService>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapGet("/", () => "Welcome in the web service of my website!");

app.MapControllers();
app.Run();
