using System.Globalization;
using GameStore;
using GameStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AuthUser = GameStore.Models.AuthUser;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<GamesDBContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<IdentityContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

builder.Services.AddControllersWithViews();
builder.Services.AddIdentity<GameStore.Models.AuthUser, IdentityRole>().AddEntityFrameworkStores<IdentityContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("uk-UA");
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("uk-UA"); 
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("uk-UA"); 

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{itemId?}");

app.Run();
