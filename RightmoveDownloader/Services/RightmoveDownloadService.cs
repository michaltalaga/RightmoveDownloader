using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using RightmoveDownloader.Clients;
using RightmoveDownloader.Repositories;

namespace RightmoveDownloader.Services
{
	public partial class RightmoveDownloadService : IRightmoveDownloadService
	{
		private readonly IRightmoveHttpClient rightmoveHttpClient;
		private readonly IPropertyRepository propertiesRepository;

		public RightmoveDownloadService(IRightmoveHttpClient rightmoveHttpClient, IPropertyRepository propertiesRepository)
		{
			this.rightmoveHttpClient = rightmoveHttpClient;
			this.propertiesRepository = propertiesRepository;
		}
		public async Task Download(string locationIdentifier, decimal radius, int minBedrooms, int maxBedrooms, int minPrice, int maxPrice)
		{
			var propertyBatches = rightmoveHttpClient.GetProperties(locationIdentifier, radius, minBedrooms, maxBedrooms, minPrice, maxPrice);
			
			await foreach (var properties in propertyBatches)
			{
				await propertiesRepository.AddProperties(properties);
			}

		}
	}
}