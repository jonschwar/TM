using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http.Headers;
using TM.TwitterClients.Monitoring;
using TM.TwitterMonitoring;

namespace TM.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                                                .SetBasePath(environmentName == "Production" ? "S:\\Somefolder" : Directory.GetCurrentDirectory())
                                                .AddJsonFile("appsettings.json", false)
                                                .AddJsonFile($"appsettings.{environmentName}.json", false)
                                                .Build();

            services.AddControllersWithViews();

            var clientSettingsSection = configuration.GetSection("TwitterClient");
            TwitterMonitorSettings settings = new TwitterMonitorSettings();
            services.Configure<TwitterMonitorSettings>(clientSettingsSection);
            ConfigurationBinder.Bind(clientSettingsSection, settings);
            services.AddSingleton<TwitterMonitorSettings>(opts => opts.GetRequiredService<IOptions<TwitterMonitorSettings>>().Value);

            services.AddHttpClient<IHashtagMonitor, HashtagMonitor>(client =>
            {
                client.BaseAddress = new Uri(settings.BaseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {settings.BearerToken}");
            });

            services.AddSingleton<HashtagDashboard>();
            services.AddSingleton<IHashtagDashboardWriter>(x => x.GetRequiredService<HashtagDashboard>());
            services.AddSingleton<IHashtagDashboardReader>(x => x.GetRequiredService<HashtagDashboard>());

            services.AddSingleton(typeof(IHashtagMonitor), typeof(HashtagMonitor));

            services.AddRazorPages();

            services.AddHostedService<TwitterMonitorService>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime hostAppLifeTime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
