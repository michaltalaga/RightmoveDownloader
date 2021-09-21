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
		const string propertiesRange = "properties!A:N";
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
			bool needsUpdate = AddOrUpdateProperties(properties, newData);
			if (!needsUpdate)
			{
				logger.LogInformation($"AddProperties(properties[{properties.Count()}]) - NOTHING TO UPDATE");
				return;
			}
			FixPropertiesFormulas(newData);
			await googleSheetsService.Update(newData, propertiesRange);
			logger.LogInformation($"AddProperties(properties[{properties.Count()}]) - DONE");
		}

		private static bool AddOrUpdateProperties(IEnumerable<RightmoveHttpClient.Property> properties, ValueRange newData)
		{
			var firstRow = newData.Values[0];
			var needsUpdate = false;
			var lastSeenTodayString = DateTime.Now.ToString("yyyy-MM-dd");

			foreach (var property in properties)
			{
				var existingEntry = newData.Values.FirstOrDefault(v => (string)v[(int)PropertyHeader.Id] == property.id);
				if (existingEntry == null)
				{
					existingEntry = new object[firstRow.Count];
					existingEntry[(int)PropertyHeader.Id] = property.id;
					existingEntry[(int)PropertyHeader.FirstSeen] = DateTime.Now.Date.ToString("yyyy-MM-dd");
					existingEntry[(int)PropertyHeader.Status] = "new";
					newData.Values.Add(existingEntry);
					needsUpdate = true;
				}
				else if ((string)existingEntry[(int)PropertyHeader.LastSeen] != lastSeenTodayString)
				{
					needsUpdate = true;
				}

				existingEntry[(int)PropertyHeader.LastSeen] = DateTime.Now.Date.ToString("yyyy-MM-dd");

				existingEntry[(int)PropertyHeader.PricePerMonth] = (property.price.frequency == "monthly" || property.price.frequency == "not specified") ? property.price.amount : property.price.amount * 4;
				existingEntry[(int)PropertyHeader.Bedrooms] = property.bedrooms;
				existingEntry[(int)PropertyHeader.NumberOfFloorPlans] = property.numberOfFloorplans;
				existingEntry[(int)PropertyHeader.Location] = property.location.latitude + "," + property.location.longitude;
				existingEntry[(int)PropertyHeader.Url] = property.propertyUrl;
			}

			return needsUpdate;
		}

		private static void FixPropertiesFormulas(ValueRange newData)
		{
			foreach (var row in newData.Values.Skip(1))
			{
				var statusCell = (string)row[(int)PropertyHeader.Status];
				if (statusCell == "new" || string.IsNullOrEmpty(statusCell))
				{
					row[(int)PropertyHeader.Status] = "=IFNA(VLOOKUP(INDIRECT(\"A\" & ROW()),statuses!A:B,2,FALSE),\"new\")";
				}
				var postCodeCell = (string)row[(int)PropertyHeader.PostCode];
				if (postCodeCell == "X" || string.IsNullOrEmpty(postCodeCell))
				{
					row[(int)PropertyHeader.PostCode] = "=IFNA(VLOOKUP(INDIRECT(\"H\" & ROW()),times!A:G,2,FALSE),\"X\")";
				}
				var transitCell = (string)row[(int)PropertyHeader.Transit];
				if (transitCell == "-1" || string.IsNullOrEmpty(transitCell))
				{
					row[(int)PropertyHeader.Transit] = "=IFNA(VLOOKUP(INDIRECT(\"H\" & ROW()),times!A:G,5,FALSE),-1)";
				}
				var walkingCell = (string)row[(int)PropertyHeader.Walking];
				if (walkingCell == "-1" || string.IsNullOrEmpty(walkingCell))
				{
					row[(int)PropertyHeader.Walking] = "=IFNA(VLOOKUP(INDIRECT(\"H\" & ROW()),times!A:G,6,FALSE),-1)";
				}
				var bicyclingCell = (string)row[(int)PropertyHeader.Bicycling];
				if (bicyclingCell == "-1" || string.IsNullOrEmpty(bicyclingCell))
				{
					row[(int)PropertyHeader.Bicycling] = "=IFNA(VLOOKUP(INDIRECT(\"H\" & ROW()),times!A:G,7,FALSE),-1)";
				}
				var minTimeCell = (string)row[(int)PropertyHeader.MinTime];
				row[(int)PropertyHeader.MinTime] = "=MIN(K2:M2)";
			}
		}

		private IList<object> GetHeaderRow()
		{
			return Enum.GetNames(typeof(PropertyHeader));
		}
		enum PropertyHeader
		{
			Id = 0,
			FirstSeen = 1,
			LastSeen = 2,
			Status = 3,
			PricePerMonth = 4,
			Bedrooms = 5,
			NumberOfFloorPlans = 6,
			Location = 7,
			Url = 8,
			PostCode = 9,
			Transit = 10,
			Walking = 11,
			Bicycling = 12,
			MinTime = 13,
		}
		public async Task<IEnumerable<string>> GetLocations(bool includeCalculated = false)
		{
			var response = await googleSheetsService.Get(propertiesRange);
			return response.Values.Skip(1).Where(v => includeCalculated || (Convert.ToInt32(v[(int)PropertyHeader.Transit]) == -1)).Select(v => (string)v[(int)PropertyHeader.Location]).Distinct().ToArray();
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