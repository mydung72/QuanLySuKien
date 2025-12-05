using EventBookingWeb.Models.DomainModels;
using EventBookingWeb.Models.Enums;
using EventBookingWeb.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IQrCodeService, QrCodeService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate(); // T? ??ng c?p nh?t DB theo migrate m?i nh?t

    // Ki?m tra xem ?� c� t�i kho?n admin ch?a
    if (!dbContext.Users.Any(u => u.Role == UserRole.Admin))
    {
        var adminUser = new DBUser
        {
            FullName = "Administrator",
            Email = "admin@gmail.com",
            PasswordHash = "$2a$12$Qdqkoks2NCM9kPo4gxTEBeEU3mTS5BahZOSmG2d9ddhDaLVWHo7c2", // ?? B?n c� th? m� ho� sau
            Role = UserRole.Admin,
            UserStatus = UserStatus.Active,
            CreatedAt = DateTime.Now
        };

        dbContext.Users.Add(adminUser);
        dbContext.SaveChanges();
        Console.WriteLine("? ?� t?o t�i kho?n admin m?c ??nh: admin@gmail.com / 123456");
    }
}

/// C?u h�nh middleware pipeline
if (app.Environment.IsDevelopment())
{
    // ?? Ch?y Swagger trong m�i tr??ng Development
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
