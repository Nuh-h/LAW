using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ArabicLearning
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // var host = CreateHostBuilder(args).Build();

            // using (var scope = host.Services.CreateScope())
            // {
            //     Console.WriteLine("About to write db");
            //     var db = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
            //     Console.WriteLine("Got it: ",db);
            //     Console.WriteLine("About to migrate");
            //     db.Database.Migrate(); // apply the migrations
            //     Console.WriteLine("Migrated successfully");
            // }

            // host.Run(); // start handling requests
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
