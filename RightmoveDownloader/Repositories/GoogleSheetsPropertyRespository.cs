using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Logging;
using RightmoveDownloader.Clients;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RightmoveDownloader.Repositories
{
	public class GoogleSheetsPropertyRespository : IPropertyRepository
	{
		private readonly IGoogleSheetsClient googleSheetsService;
		private readonly ILogger<GoogleSheetsPropertyRespository> logger;
		const string propertiesRange = "properties!A:J";
		const string travelTimesRange = "times!A:C";
		public GoogleSheetsPropertyRespository(IGoogleSheetsClient googleSheetsService, ILogger<GoogleSheetsPropertyRespository> logger)
		{
			this.googleSheetsService = googleSheetsService;
			this.logger = logger;
		}
		public async Task AddProperties(IEnumerable<RightmoveHttpClient.Property> properties)
		{
			logger.LogInformation($"AddProperties(properties[{properties.Count()}])");
			var response = await googleSheetsService.Get(propertiesRange);
			var newData = new ValueRange();
			newData.Values = response.Values ?? new List<IList<object>>();
			var firstRow = newData.Values[0];
			foreach (var property in properties)
			{
				var existingEntry = newData.Values.SingleOrDefault(v => (string)v[0] == property.id);
				if (existingEntry == null)
				{
					existingEntry = new object[firstRow.Count];
					existingEntry[2] = "new";
					newData.Values.Add(existingEntry);
				}
				existingEntry[0] = property.id;
				existingEntry[1] = DateTime.Now;
				//2 status
				existingEntry[3] = property.price.frequency == "monthly" ? property.price.amount : property.price.amount * 4;
				existingEntry[4] = property.bedrooms;
				existingEntry[5] = property.numberOfFloorplans;
				existingEntry[6] = property.location.latitude + "," + property.location.longitude;
				existingEntry[7] = property.propertyUrl;
			}
			foreach (var row in newData.Values.Skip(1))
			{
				row[8] = "=IFNA(VLOOKUP(INDIRECT(\"G\" & ROW()),times!A:C,2,FALSE),-1)";
			}
			await googleSheetsService.Update(newData, propertiesRange);
			logger.LogInformation($"AddProperties(properties[{properties.Count()}]) - DONE");
		}
		public async Task<IEnumerable<string>> GetLocations(bool includeCalculated = false)
		{
			var response = await googleSheetsService.Get(propertiesRange);
			return response.Values.Skip(1).Where(v => includeCalculated || (Convert.ToInt32(v[8]) == -1)).Select(v => (string)v[6]).Distinct().ToArray();
		}

		public async Task AddTravelTimes(IEnumerable<IGoogleMapsDistanceApiClient.TravelTime> travelTimes)
		{
			logger.LogInformation($"AddTravelTimes(travelTimes[{travelTimes.Count()}])");
			var newData = new ValueRange
			{
				Values = new List<IList<object>>()
			};
			foreach (var travelTime in travelTimes)
			{
				newData.Values.Add(new List<object>
				{
					travelTime.From,
					travelTime.Minutes,
					null
				});
			}
			await googleSheetsService.Append(newData, travelTimesRange);
			logger.LogInformation($"AddTravelTimes(travelTimes[{travelTimes.Count()}]) - DONE");
		}
	}
}