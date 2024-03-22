using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System;
using Web_PizzaShop.Hubs;
using Web_PizzaShop.Interface.Admin;
using Web_PizzaShop.Interface.Common;
using Web_PizzaShop.Interface.Public;
using Web_PizzaShop.Models;
using Web_PizzaShop.ServiceManager;
using Web_PizzaShop.ServiceManager.Admin;
using Web_PizzaShop.ServiceManager.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddSession(opt => opt.IdleTimeout = TimeSpan.FromMinutes(30));
builder.Services.AddSignalR();

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddScoped<IUserService, UserService>();
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
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("BTP_CORS");
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
// app.UseEndpoints(endpoints =>
// {
//     endpoints.MapRazorPages();
//     endpoints.MapHub<HubService>("/HubService");
//     endpoints.MapGet("/", async context =>
//     {
//         context.Response.Redirect("/Admin/Dashboard");
//     });
// });
app.Run();