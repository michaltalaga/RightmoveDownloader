using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using RightmoveDownloader.Clients;
using RightmoveDownloader.Repositories;
using Microsoft.Extensions.Logging;

namespace RightmoveDownloader.Services
{
	public partial class RightmoveDownloadService : IRightmoveDownloadService
	{
		private readonly IRightmoveHttpClient rightmoveHttpClient;
		private readonly IPropertyRepository propertiesRepository;
		private readonly ILogger<RightmoveDownloadService> logger;

		public RightmoveDownloadService(IRightmoveHttpClient rightmoveHttpClient, IPropertyRepository propertiesRepository, ILogger<RightmoveDownloadService> logger)
		{
			this.rightmoveHttpClient = rightmoveHttpClient;
			this.propertiesRepository = propertiesRepository;
			this.logger = logger;
		}
		public async Task Download(string locationIdentifier, int radius, int minBedrooms, int maxBedrooms, int minPrice, int maxPrice, string channel)
		{
			logger.LogInformation($"Download({locationIdentifier}, {radius}, {minBedrooms}, {maxBedrooms}, {minPrice}, {maxPrice})");
			var propertyBatches = rightmoveHttpClient.GetProperties(locationIdentifier, radius, minBedrooms, maxBedrooms, minPrice, maxPrice, channel);
			await foreach (var properties in propertyBatches)
			{
				await propertiesRepository.AddProperties(properties);
			}
			logger.LogInformation($"Download({locationIdentifier}, {radius}, {minBedrooms}, {maxBedrooms}, {minPrice}, {maxPrice}) - DONE");
		}
	}
}