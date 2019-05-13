using Google.Apis.Sheets.v4.Data;

namespace RightmoveDownloader.Services
{
	public interface IGoogleSheetsService
	{
		ValueRange Get(string range);
		UpdateValuesResponse Update(ValueRange body, string range);
	}
}