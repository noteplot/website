﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;    //.DefaultContractResolver
using NotePlot.Models;

namespace NotePlot
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
            string connection = Configuration.GetConnectionString("NP"); // NP - название коннекта в appsettings.json
            //services.AddDbContext<NPContext>(options => options.UseSqlServer(connection)); // Entity Framework
            //services.AddTransient<IRepositoryParameter, RepositoryParameter>(provider => new RepositoryParameter(connection));  // Dapper
            services.AddTransient<IRepositoryLogin, RepositoryLogin>(provider => new RepositoryLogin(connection));              // Dapper - это login
            services.AddTransient<IUnitCategoryRepository, UnitCategoryRepository>(provider => new UnitCategoryRepository(connection));
            services.AddTransient<IRepositoryParamValueType, RepositoryParamValueType>(provider => new RepositoryParamValueType(connection));
            
            // Add framework services.
            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver()); // ЭТО ОБЯЗАТЕЛЬНО, т.к. formatter по-умолчанию преобразует начальные символы ключей в lowecase             
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // аутентификация куки
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "NotePlotCookies", // случайное название
                LoginPath = new Microsoft.AspNetCore.Http.PathString("/Login/LoginInput"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });

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
            
            app.UseStatusCodePages(); // обработка ошибок HTTP

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
