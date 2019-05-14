using System.Threading.Tasks;

namespace RightmoveDownloader.Clients
{
	public interface IGoogleMapsDistanceApiClient
	{
		Task<int> GetMinutesBetweenPoints(string fromLocation, string toLocation);
	}
}