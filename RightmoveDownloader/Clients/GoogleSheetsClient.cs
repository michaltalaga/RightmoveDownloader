﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RightmoveDownloader.Clients
{
	public class GoogleSheetsClient : IGoogleSheetsClient
	{
		private readonly string spreadsheetId;
		SheetsService sheetsService;

		public GoogleSheetsClient(string credentialJson, string applicationName, string spreadsheetId)
		{
			var credential = GoogleCredential.FromJson(credentialJson).CreateScoped(new[] { SheetsService.Scope.Spreadsheets });
			sheetsService = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = credential, ApplicationName = applicationName });
			this.spreadsheetId = spreadsheetId;
		}

		public Task<ValueRange> Get(string range)
		{
			var getRequest = sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
			return getRequest.ExecuteAsync();
		}
		public Task<UpdateValuesResponse> Update(ValueRange body, string range)
		{
			var updateRequest = sheetsService.Spreadsheets.Values.Update(body, spreadsheetId, range);
			updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
			return updateRequest.ExecuteAsync();
		}
		public Task<AppendValuesResponse> Append(ValueRange body, string range)
		{
			var request = sheetsService.Spreadsheets.Values.Append(body, spreadsheetId, range);
			request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
			return request.ExecuteAsync();
		}
	}
}