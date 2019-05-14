using System.Collections.Generic;

namespace RightmoveDownloader.Clients
{
	public interface IRightmoveHttpClient
	{
		IAsyncEnumerable<IEnumerable<RightmoveHttpClient.Property>> GetProperties(string locationIdentifier, decimal radius, int minBedrooms, int maxBedrooms, int minPrice, int maxPrice);
	}
}