using System;
using System.Diagnostics;
using System.Threading.Tasks;

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
			var properties = rightmoveHttpClient.GetProperties(locationIdentifier, radius, minBedrooms, maxBedrooms, minPrice, maxPrice);

			await foreach (var property in properties)
			{
				propertiesRepository.AddProperty(property);
			}

		}
	}
}