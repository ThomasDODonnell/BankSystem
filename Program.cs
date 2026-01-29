using BankSystem.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddDbContext<BankSystemContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
}
);

builder.Services.AddIdentityApiEndpoints<IdentityUser>().AddEntityFrameworkStores<BankSystemContext>();
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("https://localhost:7015", "http://localhost:5234"); // These urls must match whatever the javascript endpoint is, not the api endpoint, so this will need to change
    });
});


var app = builder.Build();

// 1. Basic configuration
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// 2. Security Pipeline (ORDER IS CRITICAL)
app.UseCors();           // CORS should usually come first to handle preflight OPTIONS requests
app.UseAuthentication(); // <--- ADD THIS HERE
app.UseAuthorization();  // Authorization stays after Authentication

// 3. Endpoints (Move these to the bottom)
app.MapGroup("/auth")
   .MapIdentityApi<IdentityUser>()
   .AllowAnonymous();
app.MapControllers();

app.Run();
