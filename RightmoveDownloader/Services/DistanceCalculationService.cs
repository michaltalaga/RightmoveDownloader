using MoreLinq;
using MoreLinq.Experimental;
using RightmoveDownloader.Clients;
using RightmoveDownloader.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RightmoveDownloader.Services
{
	public class DistanceCalculationService : IDistanceCalculationService
	{
		private readonly IPropertyRepository propertyRepository;
		private readonly IGoogleMapsDistanceApiClient googleMapsDistanceApiClient;

		public DistanceCalculationService(IPropertyRepository propertyRepository, IGoogleMapsDistanceApiClient googleMapsDistanceApiClient)
		{
			this.propertyRepository = propertyRepository;
			this.googleMapsDistanceApiClient = googleMapsDistanceApiClient;
		}
		public async Task FindDistances(string toLocation)
		{
			var locations = await propertyRepository.GetLocations(false);
            foreach (var locationsBatch in locations.Batch(10))
            {
				var travelTimes = await Task.WhenAll(locationsBatch.Select(location => googleMapsDistanceApiClient.GetTravelTime(location, toLocation)));
				await propertyRepository.AddTravelTimes(travelTimes);
			}
		}
	}
}