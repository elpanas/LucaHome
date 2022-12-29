using ProvaRest.Models;
using ProvaRest.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

// Add services to the container.
builder.Services.Configure<CommentDatabaseSettings>(
    builder.Configuration.GetSection("CommentDatabase"));

builder.Services.AddSingleton<CommentService>();

builder.Services.AddControllers();

var app = builder.Build();

//app.UseHttpsRedirection();

app.UseCors(options =>
    options.WithOrigins("https://lucapanariello.altervista.org")
           .WithMethods("POST", "PUT", "DELETE", "GET")
           .AllowAnyHeader());

app.UseAuthorization();
app.MapControllers();
app.Run();
