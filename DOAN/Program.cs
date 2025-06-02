using DOAN.Controllers;
using DOAN.Data;
using DOAN.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
// Đăng ký EthereumService để có thể sử dụng Dependency Injection
builder.Services.AddSingleton<EthereumService>();

// Các cấu hình khác
builder.Services.AddControllersWithViews();

// Thêm các dịch vụ vào container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ShopConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Cấu hình Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Cấu hình JSON
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

// Lấy thông tin cấu hình trong tập tin appsettings.json và gán vào đối tượng MailSettings
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));


// Cấu hình Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "ABC";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

// Thêm dịch vụ MVC
builder.Services.AddControllersWithViews();
// Thêm TempData
builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider();

// Mã hóa mật khẩu cho mô hình tùy chỉnh
builder.Services.AddSingleton<IPasswordHasher<Nguoidung>, PasswordHasher<Nguoidung>>();

var app = builder.Build();

// Xử lý ngoại lệ
app.UseDeveloperExceptionPage();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthorization();

// Cấu hình route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Customer}/{action=Index}/{id?}");

app.Run();
