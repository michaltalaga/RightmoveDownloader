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
		const string propertiesRange = "properties!A:L";
		const string travelTimesRange = "times!A:G";
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
			if (newData.Values.Count == 0) newData.Values.Add(GetHeaderRow());
			var firstRow = newData.Values[0];
			var needsUpdate = false;
			var lastSeenTodayString = DateTime.Now.ToString("yyyy-MM-dd");

			foreach (var property in properties)
			{
				var existingEntry = newData.Values.SingleOrDefault(v => (string)v[0] == property.id);
				if (existingEntry == null)
				{
					existingEntry = new object[firstRow.Count];
					existingEntry[0] = property.id;
					existingEntry[2] = "new";
					newData.Values.Add(existingEntry);
					needsUpdate = true;
				}
				else if ((string)existingEntry[1] != lastSeenTodayString)
				{
					needsUpdate = true;
				}
				
				existingEntry[1] = DateTime.Now.Date.ToString("yyyy-MM-dd"); //1 LastSeen
																			 //2 Status
				existingEntry[3] = property.price.frequency == "monthly" ? property.price.amount : property.price.amount * 4; // PricePerMonth
				existingEntry[4] = property.bedrooms;
				existingEntry[5] = property.numberOfFloorplans;
				existingEntry[6] = property.location.latitude + "," + property.location.longitude;
				existingEntry[7] = property.propertyUrl;
			}
			if (!needsUpdate)
			{
				logger.LogInformation($"AddProperties(properties[{properties.Count()}]) - NOTHING TO UPDATE");
				return;
			}
			foreach (var row in newData.Values.Skip(1))
			{
				var postCodeCell = (string)row[8];
				if (postCodeCell == "X" || string.IsNullOrEmpty(postCodeCell))
				{
					row[8] = "=IFNA(VLOOKUP(INDIRECT(\"G\" & ROW()),times!A:G,2,FALSE),\"X\")";
				}
				var transitCell = (string)row[9];
				if (transitCell == "-1" || string.IsNullOrEmpty(transitCell))
				{
					row[9] = "=IFNA(VLOOKUP(INDIRECT(\"G\" & ROW()),times!A:G,5,FALSE),-1)";
				}
				var walkingCell = (string)row[10];
				if (walkingCell == "-1" || string.IsNullOrEmpty(walkingCell))
				{
					row[10] = "=IFNA(VLOOKUP(INDIRECT(\"G\" & ROW()),times!A:G,6,FALSE),-1)";
				}
				var bicyclingCell = (string)row[11];
				if (bicyclingCell == "-1" || string.IsNullOrEmpty(bicyclingCell))
				{
					row[11] = "=IFNA(VLOOKUP(INDIRECT(\"G\" & ROW()),times!A:G,7,FALSE),-1)";
				}
			}
			await googleSheetsService.Update(newData, propertiesRange);
			logger.LogInformation($"AddProperties(properties[{properties.Count()}]) - DONE");
		}

		private IList<object> GetHeaderRow()
		{
			return new[]
			{
				//0    1           2         3                4           5                     6           7      8           9          10         11
				"Id", "LastSeen", "Status", "PricePerMonth", "Bedrooms", "NumberOfFloorPlans", "Location", "Url", "PostCode", "Transit", "Walking", "Bicycling"
			};
		}

		public async Task<IEnumerable<string>> GetLocations(bool includeCalculated = false)
		{
			var response = await googleSheetsService.Get(propertiesRange);
			return response.Values.Skip(1).Where(v => includeCalculated || (Convert.ToInt32(v[8]) == -1)).Select(v => (string)v[6]).Distinct().ToArray();
		}

		public async Task AddTravelTimes(IEnumerable<IGoogleMapsDistanceApiClient.TravelInfo> travelTimes)
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
					travelTime.FromPostCode,
					travelTime.To,
					travelTime.ToPostCode,
					travelTime.TransitMinutes,
					travelTime.WalkingMinutes,
					travelTime.BicyclingMinutes
				});
			}
			await googleSheetsService.Append(newData, travelTimesRange);
			logger.LogInformation($"AddTravelTimes(travelTimes[{travelTimes.Count()}]) - DONE");
		}
	}
}