using E_Greetings.Authorization;
using E_Greetings.Middleware;
using E_Greetings.Models;
using E_Greetings.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var Provider = builder.Services.BuildServiceProvider();
var Config = Provider.GetRequiredService<IConfiguration>();
builder.Services.AddDbContext<EGreetingsContext>(option => option.UseSqlServer(Config.GetConnectionString("dbcs")));
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication("MyAuth")
    .AddCookie("MyAuth", options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });


builder.Services.AddAuthorization();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddAuthorization(options =>
{
    using (var scope = builder.Services.BuildServiceProvider().CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<EGreetingsContext>();

        var permissions = dbContext.Permissions.Select(p => p.Name).ToList();

        if(permissions != null)
        {
            foreach (var permission in permissions)
            {
                options.AddPolicy(permission, policy =>
                    policy.Requirements.Add(new PermissionRequirement(permission)));
            }
        }
    }
});



builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddSingleton(new MailService(
    smtpHost: "smtp.gmail.com",
    smtpPort: 587,
    smtpUser: "abdullahsheikhmuhammad21@gmail.com",
    smtpPass: "etwrtyfjxyqsbzyu"
));


var app = builder.Build();




if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMiddleware<NavbarCards>();
app.UseMiddleware<AutoDeactiveSubscribers>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Website}/{action=Home}/{id?}");

app.Run();
