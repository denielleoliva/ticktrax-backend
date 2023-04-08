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
                                builder.WithOrigins("http://localhost:5095", "http://localhost:8080")
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

        // private void createRolesandUsers()    
        // {    
        //     DbContext context = new DbContext();    
        
        //     var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));    
        //     var UserManager = new UserManager<User>(new UserStore<User>(context));    
        
        
        //     // In Startup iam creating first Admin Role and creating a default Admin User     
        //     if (!roleManager.RoleExists("Admin"))    
        //     {    
        
        //         // first we create Admin rool    
        //         var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();    
        //         role.Name = "Admin";    
        //         roleManager.Create(role);    
        
        //         //Here we create a Admin super user who will maintain the website                   
        
        //         var user = new ApplicationUser();    
        //         user.UserName = "shanu";    
        //         user.Email = "syedshanumcain@gmail.com";    
        
        //         string userPWD = "A@Z200711";    
        
        //         var chkUser = UserManager.Create(user, userPWD);    
        
        //         //Add default User to Role Admin    
        //         if (chkUser.Succeeded)    
        //         {    
        //             var result1 = UserManager.AddToRole(user.Id, "Admin");    
        
        //         }    
        //     }    
        
        //     // creating Creating Manager role     
        //     if (!roleManager.RoleExists("Manager"))    
        //     {    
        //         var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();    
        //         role.Name = "Manager";    
        //         roleManager.Create(role);    
        
        //     }    
        
        //     // creating Creating Employee role     
        //     if (!roleManager.RoleExists("Employee"))    
        //     {    
        //         var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();    
        //         role.Name = "Employee";    
        //         roleManager.Create(role);    
        
        //     }    
        //    }  
    }
}