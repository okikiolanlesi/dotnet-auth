using AuthUnderTheHood.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddAuthentication("Cookie").AddCookie("Cookie", options =>
{
    options.Cookie.Name = "Cookie";
    options.LoginPath = "/account/login";
    options.ExpireTimeSpan = TimeSpan.FromSeconds(3600);
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HR", policy =>
    {
        policy.RequireClaim("Department", "HR").Requirements.Add(new HRManagerProbationRequirement(1));
    });
});

builder.Services.AddSingleton<IAuthorizationHandler, HRManagerProbationRequirementHandler>();

builder.Services.AddHttpClient("mywebapi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5268/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");

});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(3600);
    options.Cookie.HttpOnly = true;
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
app.UseSession();
app.MapRazorPages();

app.Run();
