var builder = WebApplication.CreateBuilder(args);

// Add support for MVC Views and Controllers
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Static files (JS and CSS)
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

// Home
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
