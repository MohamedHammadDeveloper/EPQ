using EPQ.Client.Conifigrations.Table.Interfaces;
using EPQ.Client.Conifigrations.Table;
using EPQ.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using EPQ.EF.Interfaces;
using HoldPoint.EF;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddDbContext<EPQContext>(options =>
          options.UseSqlServer(
             //builder.Configuration.GetConnectionString("DefaultConnection"),
             builder.Configuration.GetConnectionString("Data Source=.\\SQLExpress;Initial Catalog=elite_erp_23_4_2024;User ID=sa;Password=01151141556;Trust Server Certificate=True"),
              b => b.MigrationsAssembly(typeof(EPQContext).Assembly.FullName)));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAccountStatementService, AccountStatementService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
