using DotNetEnv;
using LucaHome.Models;
using LucaHome.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Env.Load(); // legge .env nella root

var key = Environment.GetEnvironmentVariable("JWT_SECRET");
var expireHours = Environment.GetEnvironmentVariable("JWT_EXPIRE_HOURS");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
            build =>
            {
                build.WithOrigins("https://lucapanariello.altervista.org", "http://lucapanariello.altervista.org");                
                // build.AllowAnyOrigin();
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

//builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<CommentService>();
builder.Services.AddSingleton<ProjectService>();
builder.Services.AddSingleton<SkillService>();
builder.Services.AddSingleton<UserService>();
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
