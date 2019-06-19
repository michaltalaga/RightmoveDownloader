using System.Threading.Tasks;

namespace RightmoveDownloader.Clients
{
	public interface IGoogleMapsDistanceApiClient
	{
		Task<TravelInfo> GetTravelInfo(string fromLocation, string toLocation);
		public class TravelInfo
		{
			public string From { get; set; }
			public string FromPostCode { get; set; }
			public string To { get; set; }
			public string ToPostCode { get; set; }
			public int TransitMinutes { get; set; }
			public int WalkingMinutes { get; set; }
			public int BicyclingMinutes { get; set; }
		}
	}
}