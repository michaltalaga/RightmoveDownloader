using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4;
using Hangfire;
using Hangfire.Common;
using Hangfire.LiteDB;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using RightmoveDownloader.Clients;
using RightmoveDownloader.Repositories;
using RightmoveDownloader.Services;
using RightmoveDownloader.Utils;

namespace RightmoveDownloader
{
	public class Startup
	{
		private readonly IConfiguration configuration;

		public Startup(IConfiguration configuration)
		{
			this.configuration = configuration;
		}
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddHttpClient<IRightmoveHttpClient, RightmoveHttpClient>(client =>
			{
				client.DefaultRequestHeaders.Add("User-Agent", "C# App");
			})
			.AddPolicyHandler(Policy.BulkheadAsync<HttpResponseMessage>(5, Int32.MaxValue))
			.AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
			{
				TimeSpan.FromSeconds(1),
				TimeSpan.FromSeconds(5),
				TimeSpan.FromSeconds(10)
			}));
			services.AddSingleton<IGoogleSheetsClient>(new GoogleSheetsClient(File.ReadAllText("google-service-account.json"), configuration.GetValue<string>("GoogleAppName"), configuration.GetValue<string>("GoogleSpreadsheetId")));
			services.AddSingleton<IGoogleMapsDistanceApiClient>(s=>new GoogleMapsDistanceApiClient(configuration.GetValue<string>("GoogleMapsApiKey"), s.GetService<IHttpClientFactory>(), s.GetService<ILogger<GoogleMapsDistanceApiClient>>()));
			services.AddTransient<IRightmoveDownloadService, RightmoveDownloadService>();
			services.AddTransient<IDistanceCalculationService, DistanceCalculationService>();
			services.AddTransient<IPointsOfInterestLookupService, PointsOfInterestLookupService>();
			services.AddTransient<IPropertyRepository, GoogleSheetsPropertyRespository>();
			services.AddHangfire(config =>
			{
				if (Directory.Exists("Hangfire"))
				{
					config.UseLiteDbStorage(Path.Combine("Hangfire", "hangfire.db"));
				}
				else
				{
					config.UseMemoryStorage();
				}				
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IRecurringJobManager recurringJobManager)
		{
			GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
			app.UseHangfireDashboard(options: new DashboardOptions
            {
                Authorization = new [] { new NoAuthHangfireFilter() }
            });
			app.UseHangfireServer();
			recurringJobManager.AddOrUpdate("Download", Job.FromExpression<IRightmoveDownloadService>(service => service.Download(configuration.GetValue<string>("LocationIdentifier"), configuration.GetValue<int>("Radius"), configuration.GetValue<int>("MinBedrooms"), configuration.GetValue<int>("MaxBedrooms"), configuration.GetValue<int>("MinPrice"), configuration.GetValue<int>("MaxPrice"))), configuration.GetValue<string>("DownloadPropertiesSchedule"));
			recurringJobManager.AddOrUpdate("Calculate Distances", Job.FromExpression<IDistanceCalculationService>(service => service.FindDistances(configuration.GetValue<string>("ToLocation"))), configuration.GetValue<string>("DownloadDistancesSchedule"));
			recurringJobManager.AddOrUpdate("Find Points of Interest", Job.FromExpression<IPointsOfInterestLookupService>(service => service.FindPointsOfInterest()), configuration.GetValue<string>("FindPointsOfInterestSchedule"));
		}
	}
}