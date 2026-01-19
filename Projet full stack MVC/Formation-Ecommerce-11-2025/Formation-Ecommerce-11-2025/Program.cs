using Formation_Ecommerce_11_2025.Core.Entities.Identity;
using Formation_Ecommerce_11_2025.Core.Interfaces.Repositories.Base;
using Formation_Ecommerce_11_2025.Core.Interfaces.Repositories;
using Formation_Ecommerce_11_2025.Infrastructure.Persistence.Repositories.Base;
using Formation_Ecommerce_11_2025.Infrastructure.Persistence.Repositories;
using Formation_Ecommerce_11_2025.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Formation_Ecommerce_11_2025.Application.Categories.Mapping;
using Formation_Ecommerce_11_2025.Mapping.Category;
using Formation_Ecommerce_11_2025.Application.Categories.Interfaces;
using Formation_Ecommerce_11_2025.Application.Categories.Services;
using Formation_Ecommerce_11_2025.Core.Not_Mapped_Entities;
using Formation_Ecommerce_11_2025.Infrastructure.External.Mailing;
using Formation_Ecommerce_11_2025.Infrastructure.Extension;
using Formation_Ecommerce_11_2025.Application.Extension;
using Formation_Ecommerce_11_2025.Mapping.Auth;
using Formation_Ecommerce_11_2025.Mapping.Product;
using Formation_Ecommerce_11_2025.Mapping.Coupon;
using Formation_Ecommerce_11_2025.Mapping.Cart;
using Formation_Ecommerce_11_2025.Mapping.Home;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

//Configuration des options d'email
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));



// Add services to the container.
builder.Services.AddRazorPages();

//Dependancy Injection 



builder.Services.AddInfrastructureRegistration(builder.Configuration);
builder.Services.AddApplicationRegistration();


builder.Services.AddAutoMapper(typeof(CategoryMappingProfile));
builder.Services.AddAutoMapper(typeof(AuthMappingProfile));
builder.Services.AddAutoMapper(typeof(ProductMappingProfile));
builder.Services.AddAutoMapper(typeof(CouponMappingProfile));
builder.Services.AddAutoMapper(typeof(CartMappingProfile));
builder.Services.AddAutoMapper(typeof(HomeMappingProfile));




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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
