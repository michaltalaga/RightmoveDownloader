using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RightmoveDownloader.Services
{
	public interface IPointsOfInterestLookupService
	{
		Task FindPointsOfInterest(IEnumerable<string> names);
	}
}