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
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("CommentDatabase"));

builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<CommentService>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapGet("/", () => "Benvenuto nel web service della mia pagina personale!");

app.MapControllers();
app.Run();
