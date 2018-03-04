using System;
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
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace NotePlot
{
    public class Startup
    {
        /*
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        */
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("NP"); // NP - название коннекта в appsettings.json
            //services.AddDbContext<NPContext>(options => options.UseSqlServer(connection)); // Entity Framework
            //services.AddTransient<IRepositoryParameter, RepositoryParameter>(provider => new RepositoryParameter(connection));  // Dapper

            // Session for CORE 2.0
            services.AddDistributedMemoryCache();
            services.AddSession();

            // установка конфигурации подключения CORE 2.0
            services.AddAuthentication("NotePlotCookies")//(CookieAuthenticationDefaults.AuthenticationScheme
                    .AddCookie("NotePlotCookies",options => //CookieAuthenticationOptions
                    {
                        options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Login/LoginInput");
                    });

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddTransient<IRepositoryLogin, RepositoryLogin>(provider => new RepositoryLogin(connection));              // Dapper - это login
            services.AddTransient<IUnitCategoryRepository, UnitCategoryRepository>(provider => new UnitCategoryRepository(connection));
            services.AddTransient<IRepositoryParamValueType, RepositoryParamValueType>(provider => new RepositoryParamValueType(connection));
            //Группы параметров и пакетов
            services.AddTransient<IRepositoryParameterGroup, RepositoryParameterGroup>(provider => new RepositoryParameterGroup(connection));
            //Параметры
            services.AddTransient<IRepositoryParameter, RepositoryParameter>(provider => new RepositoryParameter(connection));
            //Ед.изм
            services.AddTransient<IRepositoryParameterUnit, RepositoryParameterUnit>(provider => new RepositoryParameterUnit(connection));
            //Монитор
            services.AddTransient<IRepositoryMonitor, RepositoryMonitor>(provider => new RepositoryMonitor(connection));
            //Мониторинг
            services.AddTransient<IRepositoryMonitoring, RepositoryMonitoring>(provider => new RepositoryMonitoring(connection));
            //Аналитика
            services.AddTransient<AnaliticTools>(provider => new AnaliticTools(connection));

            // Add framework services.
            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver()); // ЭТО ОБЯЗАТЕЛЬНО, т.к. formatter по-умолчанию преобразует начальные символы ключей в lowecase             
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            /*
            //OBSOLETE
            // аутентификация куки CORE 1.0
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "NotePlotCookies", // случайное название
                LoginPath = new Microsoft.AspNetCore.Http.PathString("/Login/LoginInput"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });
            */
            app.UseAuthentication(); //чтобы аутентификация была встроена в конвейер обработки запроса

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
            app.UseSession();         // встраивание сессий в конвейер обработки запроса  
            // устанавливаем культуру по-умолчанию
            var supportedCultures = new[]
            {
                //new CultureInfo("en-US")//,
                new CultureInfo("ru-RU")
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ru-RU"), //"en-US"
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
