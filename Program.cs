using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;

namespace ticktrax_backend 
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = BuildWebHost(args);

            if (args.Length == 1 && args[0].ToLower() == "/seed")
            {
                //RunSeeding(app);
            }

            app.Run();
        }



        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration(AddConfiguration)
                .UseStartup<Startup>()
                .Build();


        private static void AddConfiguration(WebHostBuilderContext ctx, IConfigurationBuilder builder)
        {
            builder.Sources.Clear(); // not necessary for normal implimentation

            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();
            
            // because add env var is after json config, an environment var will override a json var
        }
    }
}