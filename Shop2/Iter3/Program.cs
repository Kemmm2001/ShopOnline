using Amazon.Extensions.NETCore.Setup;
using Iter3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Lấy chuỗi kết nối từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("G3_PerfumeShopDB_Test_TestConnection");

// Đăng ký DbContext với chuỗi kết nối
builder.Services.AddDbContext<G3_PerfumeShopDB_Iter3Context>(options =>
    options.UseSqlServer(connectionString));

// Đăng ký AWSOptions với các giá trị từ appsettings.json
builder.Services.Configure<AWSOptions>(builder.Configuration.GetSection("AWS"));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
