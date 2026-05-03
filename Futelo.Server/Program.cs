using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Futelo.Server.Data;
using Futelo.Server.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FuteloContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<FuteloContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
