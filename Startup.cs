using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ticktrax_backend.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using System.Security.Claims;

namespace ticktrax_backend
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration _cnf)
        {
            Configuration = _cnf;
        }


        

        public void ConfigureServices(IServiceCollection services)
        {
            //Uncomment for VM Usage
            services.AddDbContext<TickTraxContext>(dbContextOptions =>
                dbContextOptions.UseMySql(
                    "Server=localhost,3306;Initial Catalog=tickTraxDb;User Id=dan;Password=supersecret!1;", 
                    ServerVersion.Create(new Version(10,11,1), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MariaDb)));

            //Uncomment for Personal dev environment usage
            // services.AddDbContext<TickTraxContext>(dbContextOptions =>
            //     dbContextOptions.UseMySql(
            //         "Server=localhost,3306;Initial Catalog=tickTraxDb;User Id=danno;Password=Danisthebest!1;", 
            //         ServerVersion.Create(new Version(10,11,1), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MariaDb)));
            

            services.AddIdentity<User, IdentityRole>(options => {
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;})
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<TickTraxContext>();

            services.AddTransient<ISubmissionService, SubmissionService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

            services.AddCors(options =>
                    {
                        options.AddDefaultPolicy(
                            builder =>
                            {
                                builder.WithOrigins("http://localhost:5095", "http://localhost:8080", "https://ticktrax.nevada.dev", "http://localhost:9001")
                                                    .AllowAnyHeader()
                                                    .AllowAnyMethod();
                            });
                    });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
                });

            services.AddAuthorization();

            services.AddScoped<ClaimsPrincipal>(s =>
                s.GetService<IHttpContextAccessor>().HttpContext.User);

            

            




            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
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

            CreateRoles(serviceProvider).Wait();

        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<User>>();
            string[] roleNames = { "Admin", "Manager", "Member" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            //Here you could create a super user who will maintain the web app
            var poweruser = new User
            {
                UserName = "admin",
                Email = "admin@admin.com",
            };

            string userPWD = "Admintest1!";
            var _user = await UserManager.FindByEmailAsync("admin@admin.com");

            if(_user == null)
            {
                    var createPowerUser = await UserManager.CreateAsync(poweruser, userPWD);
                    if (createPowerUser.Succeeded)
                    {
                        //here we tie the new user to the role
                        await UserManager.AddToRoleAsync(poweruser, "Admin");

                    }
            }
        }
    }}