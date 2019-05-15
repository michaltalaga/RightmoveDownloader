using System.Threading.Tasks;

namespace RightmoveDownloader.Clients
{
	public interface IGoogleMapsDistanceApiClient
	{
		Task<TravelTime> GetTravelTime(string fromLocation, string toLocation);
		public class TravelTime
		{
			public string From { get; set; }
			public string To { get; set; }
			public int Minutes { get; set; }
		}
	}
}