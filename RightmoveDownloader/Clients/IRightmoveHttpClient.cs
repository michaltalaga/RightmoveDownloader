using System.Collections.Generic;

namespace RightmoveDownloader.Clients
{
	public interface IRightmoveHttpClient
	{
		IAsyncEnumerable<IEnumerable<RightmoveHttpClient.Property>> GetProperties(string locationIdentifier, int radius, int minBedrooms, int maxBedrooms, int minPrice, int maxPrice, string channel);
	}
}