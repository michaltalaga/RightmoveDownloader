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
        const string travelTimesRange = "times!A:b";
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
                existingEntry[1] = DateTime.Now.Date.ToString("yyyy-MM-dd"); //LastSeen
                                                 //1 LastSeen
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
            foreach (var row in newData.Values.Skip(1).Where(r => (string)r[8] == "-1" || string.IsNullOrEmpty((string)r[8])))
            {
                row[8] = "=IFNA(VLOOKUP(INDIRECT(\"G\" & ROW()),times!A:C,2,FALSE),-1)";
            }
            await googleSheetsService.Update(newData, propertiesRange);
            logger.LogInformation($"AddProperties(properties[{properties.Count()}]) - DONE");
        }

        private IList<object> GetHeaderRow()
        {
            return new[]
            {
                "Id", "LastSeen", "Status", "PricePerMonth", "Bedrooms", "NumberOfFloorPlans", "Location", "Url", "Distance"
            };
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