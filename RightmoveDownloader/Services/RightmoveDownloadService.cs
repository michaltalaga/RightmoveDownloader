using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RightmoveDownloader.Services
{
	public partial class RightmoveDownloadService : IRightmoveDownloadService
	{
		private readonly IRightmoveHttpClient rightmoveHttpClient;

		public RightmoveDownloadService(IRightmoveHttpClient rightmoveHttpClient)
		{
			this.rightmoveHttpClient = rightmoveHttpClient;
		}
		public async Task Download(string locationIdentifier, decimal radius, int minBedrooms, int maxBedrooms, int minPrice, int maxPrice)
		{
			var properties = rightmoveHttpClient.GetProperties(locationIdentifier, radius, minBedrooms, maxBedrooms, minPrice, maxPrice);

			await foreach (var property in properties)
			{
				Console.WriteLine(property.price + "\t" + property.propertyUrl);

			}

		}
	}
}