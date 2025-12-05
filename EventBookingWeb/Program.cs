using EventBookingWeb.Models.DomainModels;
using EventBookingWeb.Models.Enums;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Thêm c?u hình k?t n?i DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// C?u hình Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate(); // T? ??ng c?p nh?t DB theo migrate m?i nh?t

    // Ki?m tra xem ?ã có tài kho?n admin ch?a
    if (!dbContext.Users.Any(u => u.Role == UserRole.Admin))
    {
        var adminUser = new DBUser
        {
            FullName = "Administrator",
            Email = "admin@gmail.com",
            PasswordHash = "123456", // ?? B?n có th? mã hoá sau
            Role = UserRole.Admin,
            UserStatus = UserStatus.Active,
            CreatedAt = DateTime.Now
        };

        dbContext.Users.Add(adminUser);
        dbContext.SaveChanges();
        Console.WriteLine("? ?ã t?o tài kho?n admin m?c ??nh: admin@gmail.com / 123456");
    }
}

/// C?u hình middleware pipeline
if (app.Environment.IsDevelopment())
{
    // ?? Ch?y Swagger trong môi tr??ng Development
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EventBooking API v1");
        c.RoutePrefix = "swagger"; // M? t?i: https://localhost:7012/swagger
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
