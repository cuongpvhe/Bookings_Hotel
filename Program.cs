using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using CloudinaryDotNet; // Import Cloudinary namespace
using CloudinaryAccount = CloudinaryDotNet.Account;
using Bookings_Hotel.Service;
using Bookings_Hotel.Util;
using Bookings_Hotel.Hubs; // Alias to avoid conflict with your Models.Account

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddRazorPages();
builder.Services.AddTransient<EmailService>();

builder.Services.AddDbContext<HotelBookingSystemContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB")));
builder.Services.AddScoped<HotelBookingSystemContext>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManagerOnly", policy => policy.RequireRole(RoleName.MANAGER));
    options.AddPolicy("StaffOnly", policy => policy.RequireRole(RoleName.STAFF));
    options.AddPolicy("NonCustomerOnly", policy => policy.RequireRole(RoleName.MANAGER, RoleName.STAFF));
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";  
        options.LogoutPath = "/Home/LogOut"; 
        options.AccessDeniedPath = "/AccessDenied"; 
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10); 
    });

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // Set 10MB file limit
});

// Configure Cloudinary using alias CloudinaryAccount to avoid conflict
var cloudinaryAccount = new CloudinaryAccount(
    builder.Configuration["Cloudinary:CloudName"],
    builder.Configuration["Cloudinary:ApiKey"],
    builder.Configuration["Cloudinary:ApiSecret"]
);

// Create Cloudinary instance
Cloudinary cloudinary = new Cloudinary(cloudinaryAccount);
builder.Services.AddSingleton(cloudinary);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
});
app.MapHub<OrderStatusHub>("/orderStatusHub");


app.Run();