using Google.Apis.Sheets.v4.Data;

namespace RightmoveDownloader.Clients
{
	public interface IGoogleSheetsClient
	{
		ValueRange Get(string range);
		UpdateValuesResponse Update(ValueRange body, string range);
	}
}