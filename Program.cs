using LucaHome.Models;
using LucaHome.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
            build =>
            {
                build.WithOrigins("https://lucapanariello.altervista.org", "http://lucapanariello.altervista.org");                
                //build.AllowAnyOrigin();
                build.AllowAnyMethod();
                build.AllowAnyHeader();                          
            });    
});

// Add services to the container.
builder.Services.Configure<DatabaseSettings>(options =>
{
    options.ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION")
                               ?? builder.Configuration["CommentDatabase:ConnectionString"];
    options.DatabaseName = Environment.GetEnvironmentVariable("DB_NAME")
                           ?? builder.Configuration["CommentDatabase:DatabaseName"];
    options.CommentCollectionName = Environment.GetEnvironmentVariable("DB_COMMENT_COLLECTION")
                                    ?? builder.Configuration["CommentDatabase:CommentCollectionName"];
    options.UserCollectionName = Environment.GetEnvironmentVariable("DB_USER_COLLECTION")
                                 ?? builder.Configuration["CommentDatabase:UserCollectionName"];
    options.ProjectCollectionName = Environment.GetEnvironmentVariable("DB_PROJECT_COLLECTION")
                                    ?? builder.Configuration["CommentDatabase:ProjectCollectionName"];
    options.SkillCollectionName = Environment.GetEnvironmentVariable("DB_SKILL_COLLECTION")
                                  ?? builder.Configuration["CommentDatabase:SkillCollectionName"];
});

builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<CommentService>();
builder.Services.AddSingleton<ProjectService>();
builder.Services.AddSingleton<SkillService>();
builder.Services.AddControllers();

var app = builder.Build();
if (!app.Environment.IsProduction())
    {
        app.UseHttpsRedirection();
    }
app.UseCors();
app.UseAuthorization();

app.MapGet("/", () => "Welcome in the web service of my website!");

app.MapControllers();
app.Run();
