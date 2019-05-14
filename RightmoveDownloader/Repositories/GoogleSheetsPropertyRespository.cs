using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
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
		const string propertiesRange = "properties!A:J";
		public GoogleSheetsPropertyRespository(IGoogleSheetsClient googleSheetsService)
		{
			this.googleSheetsService = googleSheetsService;
		}
		public void AddProperties(IEnumerable<RightmoveHttpClient.Property> properties)
		{
			var response = googleSheetsService.Get(propertiesRange);
			var newData = new ValueRange();
			newData.Values = response.Values ?? new List<IList<object>>();
			var firstRow = newData.Values[0];
			int row = 2;
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
				existingEntry[8] = "=IFNA(VLOOKUP(INDIRECT(\"G\" & ROW()),distances!A:C,2,FALSE),-1)";
				row++;
			}
			googleSheetsService.Update(newData, propertiesRange);
		}
		public IEnumerable<string> GetLocations(bool includeCalculated = false)
		{
			var response = googleSheetsService.Get(propertiesRange);
			return response.Values.Skip(1).Where(v => includeCalculated || (Convert.ToInt32(v[8]) == -1)).Select(v => (string)v[6]).Distinct().ToArray();
		}
	}
}