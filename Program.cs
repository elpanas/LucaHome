using DotNetEnv;
using LucaHome.Factories;
using LucaHome.Mappers;
using LucaHome.Models;
using LucaHome.Repositories;
using LucaHome.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

Env.Load(); // legge .env nella root

// JWT SETTINGS
var key = Environment.GetEnvironmentVariable("JWT_SECRET");
var expireHours = Environment.GetEnvironmentVariable("JWT_EXPIRE_HOURS");
// -------------------------------------------

// PROBLEM DETAILS
builder.Services.AddProblemDetails();

// RATE LIMITER
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("strict", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(5); // Finestra di 5 minuti
        opt.PermitLimit = 2; // Massimo 2 messaggi per finestra
        opt.QueueLimit = 0;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
// -------------------------------------------

// JWT AUTHENTICATION
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
// -------------------------------------------

builder.Services.AddAuthorization();

// CORS
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
// -------------------------------------------

// DATABASE SETTINGS
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
// -------------------------------------------

// REPOSITORIES
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<SkillRepoMongo>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// FACTORIES
builder.Services.AddScoped<ISkillFactory, SkillFactory>();
// builder.Services.AddScoped<SkillRepoMongo>(); // registrazione concreta per la factory

// SERVICES
builder.Services.AddScoped<ICommentService,CommentService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ProjectService>();
// -------------------------------------------

// CONTROLLERS
builder.Services.AddControllers();

// AUTOMAPPER
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<CommentMapper>();
});
// -------------------------------------------

builder.Services.AddOutputCache(options =>
{
    // Opzionale: definisci una policy globale o specifica
    options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromMinutes(10)));
});

var app = builder.Build();

if (app.Environment.IsProduction())
    {
        app.UseStatusCodePages();
        app.UseExceptionHandler(); // Middleware per gestire le eccezioni globalmente
        app.UseHsts();
    }
else
    {
        app.UseHttpsRedirection();
        app.UseDeveloperExceptionPage(); // Mostra dettagli degli errori in sviluppo
    }

app.UseCors();
app.UseOutputCache();
app.UseRateLimiter();
app.UseAuthorization();
app.MapGet("/", () => "Welcome in the web service of my website!");
app.MapControllers();

app.Run();
