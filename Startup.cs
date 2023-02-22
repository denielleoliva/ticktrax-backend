using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ticktrax_backend.Data;

namespace ticktrax_backend
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TickTraxContext>(dbContextOptions =>
                dbContextOptions.UseMySql(
                    "Server=localhost,3306;Initial Catalog=tickTraxDb;User Id=dan;Password=supersecret!1;", 
                    ServerVersion.Create(new Version(10,11,1), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MariaDb)));
            

            services.AddIdentity<User, IdentityRole>(options => {
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLength = 6;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;})
                .AddEntityFrameworkStores<TickTraxContext>();

            services.AddTransient<ISubmissionService, SubmissionService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
            services.AddScoped<ITokenCreationService, JwtService>();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:5095", "http://localhost:8080")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                    }
                );
            });

            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            
            app.UseAuthorization();

            app.UseCors();

            app.UseEndpoints(cfg =>
            {
                cfg.MapControllerRoute(
                    name: "Defualt",
                    pattern: "/{controller=Home}/{action=Index}/{id?}");
            });

            
        }
    }
}