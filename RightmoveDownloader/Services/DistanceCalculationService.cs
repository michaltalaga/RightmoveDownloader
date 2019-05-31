using Microsoft.Extensions.Logging;
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
		private readonly ILogger<DistanceCalculationService> logger;

		public DistanceCalculationService(IPropertyRepository propertyRepository, IGoogleMapsDistanceApiClient googleMapsDistanceApiClient, ILogger<DistanceCalculationService> logger)
		{
			this.propertyRepository = propertyRepository;
			this.googleMapsDistanceApiClient = googleMapsDistanceApiClient;
			this.logger = logger;
		}
		public async Task FindDistances(string toLocation)
		{
			logger.LogInformation($"FindDistances({toLocation})");
			var locations = await propertyRepository.GetLocations(false);
			logger.LogInformation($"FindDistances({toLocation}|locations[{locations.Count()}])");
            foreach (var locationsBatch in locations.Batch(10))
            {
				var travelTimes = await Task.WhenAll(locationsBatch.Select(location => googleMapsDistanceApiClient.GetTravelInfo(location, toLocation)));
				await propertyRepository.AddTravelTimes(travelTimes);
			}
			logger.LogInformation($"FindDistances({toLocation}|locations[{locations.Count()}]) - DONE");
		}
	}
}