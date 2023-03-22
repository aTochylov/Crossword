using CrosswordCreator.Data;
using CrosswordCreator.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ICreateCrosswordService, CreateCrosswordService>();
builder.Services.AddTransient<IDbCrosswordService, DbCrosswordService>();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<CrosswordDbContext>(options =>
        //options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CrosswordDb"));
        options.UseSqlite($"Data Source={builder.Environment.ContentRootPath}/crossword.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Create/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Crossword}/{action=Index}/{code?}");

app.Run();
