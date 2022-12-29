using ProvaRest.Models;
using ProvaRest.Services;

var builder = WebApplication.CreateBuilder(args);
var  MyCorPolicy = "_myCorPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyCorPolicy,
                      policy =>
                      {                          
                          policy.WithOrigins("https://lucapanariello.altervista.org")                 
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                      });    
});

// Add services to the container.
builder.Services.Configure<CommentDatabaseSettings>(
    builder.Configuration.GetSection("CommentDatabase"));

builder.Services.AddSingleton<CommentService>();

builder.Services.AddControllers();

var app = builder.Build();

//app.UseHttpsRedirection();
app.UseCors("MyCorPolicy");
app.UseAuthorization();

app.MapGet("/", () => "Benvenuto nel web service della mia pagina personale!");

app.MapControllers();
app.Run();
