using RightmoveDownloader.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RightmoveDownloader.Repositories
{
	public interface IPropertyRepository
	{
		void AddProperties(IEnumerable<RightmoveHttpClient.Property> properties);
		IEnumerable<string> GetLocations(bool includeCalculated = false);
	}
}