using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RightmoveDownloader.Clients
{
	public interface IGooglePlacesApiClient
	{
		Task<object> FindNearestPlace(string location, string name);
	}
}