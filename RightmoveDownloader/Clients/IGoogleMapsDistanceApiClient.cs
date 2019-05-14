namespace RightmoveDownloader.Clients
{
	public interface IGoogleMapsDistanceApiClient
	{
		int GetMinutesBetweenPoints(string fromLocation, string toLocation);
	}
}