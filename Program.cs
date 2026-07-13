using DotNetEnv;
using LucaHome.DBs;
using LucaHome.Factories;
using LucaHome.Mappers;
using LucaHome.Models;
using LucaHome.Repositories;
using LucaHome.Repositories.Mongo;
using LucaHome.Repositories.SQL;
using LucaHome.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Env.Load(); // legge .env nella root

// JWT SETTINGS
var key = Environment.GetEnvironmentVariable("JWT_SECRET");
var expireHours = Environment.GetEnvironmentVariable("JWT_EXPIRE_HOURS");
// -------------------------------------------

// PROBLEM DETAILS
builder.Services.AddProblemDetails();

// PROTEZIONE DATI TEMPORANEI
builder.Services.AddDataProtection().UseEphemeralDataProtectionProvider(); 

// RATE LIMITER
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("strict", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(5); // Finestra di 5 minuti
        opt.PermitLimit = 2; // Massimo 2 messaggi per finestra
        opt.QueueLimit = 0; // Non accodare richieste in eccesso
        //opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
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
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"), // emesso dalla mia app
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"), // destinatari previsti (client)
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
        };
    });
// -------------------------------------------

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(build =>
    {
        build.WithOrigins("https://lucapanariello.altervista.org", "http://lucapanariello.altervista.org")
        //build.AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader();
    }); 
});
// -------------------------------------------

// DATABASE SETTINGS MONGO
builder.Services.Configure<DBSettings>(options =>
{
    options.ConnectionStringMongo = Environment.GetEnvironmentVariable("DB_CONNECTION")
                               ?? builder.Configuration["DatabaseSettings:Mongo:ConnectionStringMongo"];
    options.DatabaseName = Environment.GetEnvironmentVariable("DB_NAME")
                           ?? builder.Configuration["DatabaseSettings:Mongo:DatabaseName"];
    options.CommentCollectionName = Environment.GetEnvironmentVariable("DB_COMMENT_COLLECTION")
                                    ?? builder.Configuration["DatabaseSettings:Mongo:CommentCollectionName"];
    options.UserCollectionName = Environment.GetEnvironmentVariable("DB_USER_COLLECTION")
                                 ?? builder.Configuration["DatabaseSettings:Mongo:UserCollectionName"];
    options.ProjectCollectionName = Environment.GetEnvironmentVariable("DB_PROJECT_COLLECTION")
                                    ?? builder.Configuration["DatabaseSettings:Mongo:ProjectCollectionName"];
    options.SkillCollectionName = Environment.GetEnvironmentVariable("DB_SKILL_COLLECTION")
                                  ?? builder.Configuration["DatabaseSettings:Mongo:SkillCollectionName"];
    options.DbProvider = Environment.GetEnvironmentVariable("DB_PROVIDER")
                        ?? builder.Configuration["DatabaseSettings:DbProvider"];
});
// -------------------------------------------

// DATABASE SETTINGS SQL
var ConnectionStringSql = Environment.GetEnvironmentVariable("DB_CONNECTION_SQL")
                            ?? builder.Configuration["DatabaseSettings:SQL:ConnectionStringSql"];

builder.Services.AddDbContext<SQLDBContext>(options =>
    options.UseSqlServer(ConnectionStringSql));
// -------------------------------------------

// REPOSITORIES
builder.Services.AddScoped<ICommentRepository, CommentRepoMongo>();
builder.Services.AddScoped<ISkillRepository, SkillRepoMongo>();
builder.Services.AddScoped<IUserRepository, UserRepoMongo>();
builder.Services.AddScoped<ICommentRepository, CommentRepoSQL>();
builder.Services.AddScoped<ISkillRepository, SkillRepoSQL>();
builder.Services.AddScoped<IUserRepository, UserRepoSQL>();

// Registrazioni concrete per le factory
builder.Services.AddScoped<CommentRepoMongo>();
builder.Services.AddScoped<SkillRepoMongo>();
builder.Services.AddScoped<UserRepoMongo>();
builder.Services.AddScoped<CommentRepoSQL>();
builder.Services.AddScoped<SkillRepoSQL>();
builder.Services.AddScoped<UserRepoSQL>();

// FACTORIES
builder.Services.AddScoped<ICommentFactory, CommentFactory>();
builder.Services.AddScoped<ISkillFactory, SkillFactory>();
builder.Services.AddScoped<IUserFactory, UserFactory>();

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
    cfg.AddProfile<SkillMapper>();
});
// -------------------------------------------

builder.Services.AddOutputCache(options =>
{
    // policy globale o specifica
    options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromMinutes(10)));
});

var app = builder.Build();

app.UseCors();

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

app.UseOutputCache();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => "Welcome in the web service of my website!");
app.MapControllers();

app.Run();

// per il testing
public partial class Program { }