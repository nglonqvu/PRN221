using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System;
using Web_PizzaShop.Hubs;
using Web_PizzaShop.Interface.Admin;
using Web_PizzaShop.Models;
using Web_PizzaShop.ServiceManager.Admin;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddSession(opt => opt.IdleTimeout = TimeSpan.FromMinutes(15));
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddDbContext<PRN221_PRJContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("PRN221_DB")));
builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
builder.Services.AddMvc();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("BTP_CORS");
app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapHub<HubService>("/HubService");
app.Run();