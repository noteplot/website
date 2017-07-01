using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NPTest1.Models;                   // пространство имен моделей
using Microsoft.EntityFrameworkCore;    // пространство имен EntityFramework
using Newtonsoft.Json.Serialization;    //.DefaultContractResolver

namespace NPTest1
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("TEST"); // TEST - название коннекта в appsettings.json
            services.AddDbContext<NPContext>(options => options.UseSqlServer(connection)); // Entity Framework
            services.AddTransient<IRepositoryParameter, RepositoryParameter>(provider => new RepositoryParameter(connection));  // Dapper
            services.AddTransient<IRepositoryLogin, RepositoryLogin>(provider => new RepositoryLogin(connection));              // Dapper - это login
            
            // Add framework services.
            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver()); // ЭТО ОБЯЗАТЕЛЬНО, т.к. formatter по-умолчанию преобразует начальные символы ключей в lowecase             
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // ViewComponents
            //app.UseStatusCodePages();
            //app.UseMvcWithDefaultRoute();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",                    
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
