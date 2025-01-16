using Microsoft.EntityFrameworkCore;
using SimpleNET.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
    logging.SetMinimumLevel(LogLevel.Debug);
});

var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
logger.LogDebug("Debug level log");
logger.LogInformation("Information level log");
logger.LogError("Error level log");

var app = builder.Build();

// DB 연결 확인 코드 추가
try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureCreated();  // DB 연결 확인
        app.Logger.LogInformation("Database connection successful.");
    }
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Database connection failed.");
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())  // 개발 환경인지 확인
{
    app.UseDeveloperExceptionPage();  // 개발 환경에서는 디버그 예외 페이지 사용
}
else
{
    app.UseExceptionHandler("/Home/Error");  // 운영 환경에서는 기본 예외 처리 페이지 사용
    app.UseHsts();  // HTTP Strict Transport Security 활성화
}

//app.UseHttpsRedirection();  // HTTPS 리디렉션
app.UseStaticFiles();  // wwwroot 폴더의 정적 파일 제공

app.UseRouting();  // 요청 라우팅 활성화

app.UseAuthorization();  // 인증 활성화

// 정적 파일 및 기본 라우팅 설정
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");  // 기본 라우팅

app.Run();  // 애플리케이션 실행
