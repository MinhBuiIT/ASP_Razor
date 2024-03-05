using ASP_RazorWeb.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
var services = builder.Services;
//services cho Entity Framework
services.AddDbContext<BlogContext>((options) => {
    string connectionString = builder.Configuration.GetConnectionString("sqlConnection");
    options.UseSqlServer(connectionString);
});
var mailSetting = builder.Configuration.GetSection("MailSetting");

services.AddOptions();
services.Configure<MailSetting>(mailSetting);
services.AddTransient<IEmailSender,MailSenderService>();
//dky dich vụ Identity
services.AddIdentity<AppUser,IdentityRole>().AddEntityFrameworkStores<BlogContext>().AddDefaultTokenProviders();
//dky dich vụ Identity sử dụng Identity UI
// services.AddDefaultIdentity<AppUser>().AddEntityFrameworkStores<BlogContext>().AddDefaultTokenProviders();

// Truy cập IdentityOptions
services.Configure<IdentityOptions> (options => {
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes (5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 5 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
    options.SignIn.RequireConfirmedAccount = true;  //yêu cầu xác thực email rồi mới cho đăng nhập

});
//Cấu hình Authorization
services.ConfigureApplicationCookie(options => {
    options.LoginPath = $"/dangnhap";
    options.LogoutPath = $"/dangxuat";
    options.AccessDeniedPath = $"/denied";
});

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

app.UseRouting();

app.UseAuthentication();
//Authorization phải đặt trước Authentication
app.UseAuthorization();

app.MapRazorPages();

app.Run();


// Có hai pthuc đăng nhập signInManager.SignInAsync(user,false)
//                         signInManager.PasswordSignInAsync(username,password,isPersistent,lockoutOnFailure)