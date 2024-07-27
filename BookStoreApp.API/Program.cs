using BookStoreApp.API.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region AddDataBase

var ConnectionString = builder.Configuration.GetConnectionString("LocalConnection");
builder.Services.AddDbContext<BookStoreAppDbContext>(options => options.UseSqlServer(ConnectionString));

#endregion

#region Logger 

builder.Host.UseSerilog((ctx, lc) =>
lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

#endregion

#region Policy

builder.Services.AddCors(options =>
options.AddPolicy("AllowAll", b => b.AllowAnyMethod().AllowAnyMethod().AllowAnyOrigin()));

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
