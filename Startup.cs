using Microsoft.EntityFrameworkCore;
using ticktrax_backend.Data;

namespace ticktrax_backend
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TickTraxContext>(dbContextOptions =>
                dbContextOptions.UseMySql(
                    "Server=localhost,3306;Initial Catalog=tickTraxDb;User Id=root;Password=password;", 
                    ServerVersion.Create(new Version(10,11,1), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MariaDb)));
            services.AddTransient<ISubmissionService, SubmissionService>();
            services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
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

            app.UseEndpoints(cfg =>
            {
                cfg.MapControllerRoute(
                    name: "Defualt",
                    pattern: "/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}