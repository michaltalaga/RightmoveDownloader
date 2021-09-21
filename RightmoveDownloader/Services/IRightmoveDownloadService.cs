using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RightmoveDownloader.Services
{
	public interface IRightmoveDownloadService
	{
		Task Download(string locationIdentifier, int radius, int minBedrooms, int maxBedrooms, int minPrice, int maxPrice, string channel);
	}
}