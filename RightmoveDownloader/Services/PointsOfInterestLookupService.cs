using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RightmoveDownloader.Services
{
	public class PointsOfInterestLookupService : IPointsOfInterestLookupService
	{
		private readonly ILogger<PointsOfInterestLookupService> logger;

		public PointsOfInterestLookupService(ILogger<PointsOfInterestLookupService> logger)
		{
			this.logger = logger;
		}

		public async Task FindPointsOfInterest(IEnumerable<string> names)
		{
			logger.LogInformation($"FindPointsOfInterest()");
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