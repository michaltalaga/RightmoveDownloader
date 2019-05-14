using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RightmoveDownloader.Services
{
	public class GoogleSheetsPropertyRespository : IPropertyRepository
	{
		private readonly IGoogleSheetsService googleSheetsService;

		public GoogleSheetsPropertyRespository(IGoogleSheetsService googleSheetsService)
		{
			this.googleSheetsService = googleSheetsService;
		}
		public void AddProperties(IEnumerable<RightmoveHttpClient.Property> properties)
		{
			String range = "properties!A1:Y";
			var response = googleSheetsService.Get(range);
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
				//status
				existingEntry[3] = property.price.frequency == "monthly" ? property.price.amount : property.price.amount * 4;
				existingEntry[4] = property.bedrooms;
				existingEntry[5] = property.numberOfFloorplans;
				existingEntry[6] = property.location.latitude + "," + property.location.longitude;
				existingEntry[7] = property.propertyUrl;
			}
			googleSheetsService.Update(newData, range);
		}
	}
}