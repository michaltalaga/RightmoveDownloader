using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RightmoveDownloader.Clients
{
	public interface IGooglePlacesApiClient
	{
		Task<object> GetPointsOfInterest(string location, params string[] names);
	}
}