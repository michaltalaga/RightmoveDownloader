using Google.Apis.Sheets.v4.Data;
using System.Threading.Tasks;

namespace RightmoveDownloader.Clients
{
	public interface IGoogleSheetsClient
	{
		Task<ValueRange> Get(string range);
		Task<UpdateValuesResponse> Update(ValueRange body, string range);
		Task<AppendValuesResponse> Append(ValueRange body, string range);
	}
}