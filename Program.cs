using AspNetCoreHero.ToastNotification;
using Business.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Business.Data;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Services;
using System.Reflection;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Supermarket.Models;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Domain;

var builder = WebApplication.CreateBuilder(args);

var connectString = builder.Configuration.GetConnectionString("WebShopConnectionString");

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddSingleton<LanguageService>();
//builder.Services.AddTransient<LanguageService>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMvc()
        .AddViewLocalization()
        .AddDataAnnotationsLocalization(options =>
        
            options.DataAnnotationLocalizerProvider = (type, factory) =>
            {
                var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
                return factory.Create(nameof(SharedResource), assemblyName.Name);
            }
        );

builder.Services.Configure<RequestLocalizationOptions>(

    options =>
    {
        var supportedCultures = new List<CultureInfo>
        {
            new CultureInfo("en-US"),
            new CultureInfo("ru-Ru"),
        };
        options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;

        options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
    });

//builder.Services.AddRazorPages();



builder.Services.AddDbContext<DbMarketsContext>(options =>
    options.UseSqlServer(connectString));

builder.Services.AddControllers(
	options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

builder.Services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));
builder.Services.AddSession();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(p =>
                {
                    p.Cookie.Name = "UserLoginCookie";
                    p.ExpireTimeSpan = TimeSpan.FromDays(1);
                    p.LoginPath = "/login.html";
                    p.LogoutPath = "/log-out/html";
                    p.AccessDeniedPath = "/not-found.html";
                });
builder.Services.AddNotyf(config => { config.DurationInSeconds = 3; config.IsDismissable = true; config.Position = NotyfPosition.TopRight; });
builder.Services.AddAutoMapper(typeof(Program).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
