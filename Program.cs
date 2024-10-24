using Bookings_Hotel.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorPages();


builder.Services.AddDbContext<HotelBookingSystemContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB")));
builder.Services.AddScoped<HotelBookingSystemContext>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";  
        options.LogoutPath = "/Home/LogOut"; 
        options.AccessDeniedPath = "/Home/AccessDenied"; 
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10); 
    });



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

app.Run();
