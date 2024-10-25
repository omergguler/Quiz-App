using Microsoft.AspNetCore.Identity.UI.Services;
using PropayTest.Services;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddTransient<PropayTest.Services.IEmailSender, EmailSender>();

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    /*options.Cookie.SameSite = SameSiteMode.Lax; // or SameSiteMode.None if needed*/
});

// Add services to the container.
builder.Services.AddRazorPages();


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

app.UseAuthorization();

// Use session middleware
app.UseSession();

app.MapRazorPages();

app.Run();
