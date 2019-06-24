using Microsoft.Extensions.Logging;
using RightmoveDownloader.Clients;
using RightmoveDownloader.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RightmoveDownloader.Services
{
	public class PointsOfInterestLookupService : IPointsOfInterestLookupService
	{
        private readonly IPropertyRepository propertyRepository;
        private readonly IGooglePlacesApiClient googlePlacesApiClient;
        private readonly ILogger<PointsOfInterestLookupService> logger;

		public PointsOfInterestLookupService(IPropertyRepository propertyRepository, IGooglePlacesApiClient googlePlacesApiClient, ILogger<PointsOfInterestLookupService> logger)
		{
            this.propertyRepository = propertyRepository;
            this.googlePlacesApiClient = googlePlacesApiClient;
            this.logger = logger;
		}

		public async Task FindPointsOfInterest(IEnumerable<string> names)
		{
			logger.LogInformation($"FindPointsOfInterest()");
            var x = await propertyRepository.GetLocations(true);
            var distance = await googlePlacesApiClient.FindNearestPlace(x.First(), names.First());
			//var locations = await propertyRepository.GetLocations(false);
			//logger.LogInformation($"FindDistances({toLocation}|locations[{locations.Count()}])");
			//foreach (var locationsBatch in locations.Batch(10))
			//{
			//	var travelTimes = await Task.WhenAll(locationsBatch.Select(location => googleMapsDistanceApiClient.GetTravelInfo(location, toLocation)));
			//	await propertyRepository.AddTravelTimes(travelTimes);
			//}
			logger.LogInformation($"FindPointsOfInterest() - DONE");
		}
	}
}