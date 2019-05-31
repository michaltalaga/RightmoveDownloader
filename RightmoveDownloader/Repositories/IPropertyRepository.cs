using RightmoveDownloader.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RightmoveDownloader.Repositories
{
	public interface IPropertyRepository
	{
		Task AddProperties(IEnumerable<RightmoveHttpClient.Property> properties);
		Task<IEnumerable<string>> GetLocations(bool includeCalculated = false);
		Task AddTravelTimes(IEnumerable<IGoogleMapsDistanceApiClient.TravelInfo> travelTimes);
	}
}