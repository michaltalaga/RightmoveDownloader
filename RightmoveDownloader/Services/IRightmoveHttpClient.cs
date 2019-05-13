using System.Collections.Generic;

namespace RightmoveDownloader.Services
{
	public interface IRightmoveHttpClient
	{
		IAsyncEnumerable<RightmoveHttpClient.Property> GetProperties(string locationIdentifier, decimal radius, int minBedrooms, int maxBedrooms, int minPrice, int maxPrice);
	}
}