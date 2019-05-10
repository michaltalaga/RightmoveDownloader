using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Common;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using RightmoveDownloader.Services;

namespace RightmoveDownloader
{
    public class Startup
    {
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
        {
			services.AddHttpClient("test", x=> { }).AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
			{
				TimeSpan.FromSeconds(1),
				TimeSpan.FromSeconds(5),
				TimeSpan.FromSeconds(10)
			}));
			services.AddTransient<IRightmoveDownloadService, RightmoveDownloadService>();
			services.AddHangfire(config =>
			{
				config.UseMemoryStorage();
			});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IRecurringJobManager recurringJobManager)
        {
			app.UseHangfireDashboard();
			app.UseHangfireServer();
			recurringJobManager.AddOrUpdate("Download", Job.FromExpression<IRightmoveDownloadService>(service => service.Download("CR0 6EF", 20, 2, 3, 1400, 1800)), Cron.Yearly(2, 31));

		}
	}
}
