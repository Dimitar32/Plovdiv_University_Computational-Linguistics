using Microsoft.EntityFrameworkCore;
using RegularExpressionTask3.Data;

var builder = WebApplication.CreateBuilder(args);

// **🔹 Configure Database Connection**
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// **🔹 Add Controllers with Views**
builder.Services.AddControllersWithViews();

var app = builder.Build();

// **🔹 Configure the HTTP request pipeline**
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// **🔹 Configure Default Routing**
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Phone}/{action=Index}/{id?}");

app.Run();
