using ProvaRest.Models;
using ProvaRest.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

// Add services to the container.
builder.Services.Configure<CommentDatabaseSettings>(
    builder.Configuration.GetSection("CommentDatabase"));

builder.Services.AddSingleton<CommentService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(options =>
    options.WithOrigins("https://lucapanariello.altervista.org")
           .WithMethods("POST", "PUT", "DELETE", "GET")
           .AllowAnyHeader());

app.UseAuthorization();
app.MapControllers();
app.Run();
