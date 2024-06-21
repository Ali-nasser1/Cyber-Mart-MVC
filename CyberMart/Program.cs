using CyberMart.BusinessLogic.Interfaces;
using CyberMart.BusinessLogic.Repositories;
using CyberMart.DataAccess.Contexts;
using CyberMart.MappingProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using CyberMart.Utilities;
using Stripe;
namespace CyberMart
{
    public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();
			builder.Services.AddDbContext<CyberMartDbContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
			});

            //builder.Services.Configure<>(builder.Configuration.GetSection("stripe"));
            builder.Services.Configure<StripeData>(builder.Configuration.GetSection("stripe"));

			builder.Services.AddIdentity<IdentityUser, IdentityRole>(
				             options => options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(4))
				            .AddDefaultTokenProviders()
							.AddDefaultUI()
				            .AddEntityFrameworkStores<CyberMartDbContext>();

			builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
			builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
			builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
			builder.Services.AddAutoMapper(M => M.AddProfile(new ProductProfile()));
            builder.Services.AddSingleton<IEmailSender, EmailSender>();
			builder.Services.AddMemoryCache();
			builder.Services.AddSession();
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

			StripeConfiguration.ApiKey = builder.Configuration.GetSection("stripe:Secretkey").Get<string>();

			app.UseAuthentication();
            app.UseAuthorization();

			app.UseSession();
			app.MapRazorPages();

			app.MapControllerRoute(
				name: "default",
				pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                 name: "Customer",
                 pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
            app.Run();
		}
	}
}
