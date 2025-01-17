using HopeBank;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration.Get<Config>()!;
var env = builder.Environment;

var db = new DbCore(config.DBHopeBank);
db.InitMigration();

builder.Services.AddSingleton(db);
builder.Services.AddControllers();
// Add services to the container.
builder.Services.AddRazorPages(opt=> { 
    opt.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
    opt.Conventions.AuthorizeFolder("/");
    opt.Conventions.AllowAnonymousToPage("/Login");
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/accessdenied";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.Cookie.IsEssential = true;
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
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
