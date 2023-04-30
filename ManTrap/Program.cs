using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Serilog;
using Serilog.Events;
using Serilog.AspNetCore;
using System;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromDays(30);
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "MyCookieAuthenticationScheme";
}).AddCookie("MyCookieAuthenticationScheme", options =>
{
    options.LoginPath = "/Pages/Authorization";
    options.LogoutPath = "/Pages/Shared/_Layout";
});

// Configure Serilog
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog(dispose: true);
});

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration);

    loggerConfiguration.Enrich.FromLogContext()
        .Enrich.WithProperty("ApplicationName", context.HostingEnvironment.ApplicationName)
        .Enrich.WithProperty("EnvironmentName", context.HostingEnvironment.EnvironmentName)
        .Enrich.WithProperty("MachineName", Environment.MachineName)
        .WriteTo.Console(new RenderedCompactJsonFormatter())
        .WriteTo.File("logs/myapp.log", rollingInterval: RollingInterval.Day);
});

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(@"D:\Учеба\4335\2_семестр\ТРПО\ЛР3\ManTrap\manga_images"),
    RequestPath = "/manga_images"
});


app.UseRouting();
app.UseAuthentication();
app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
