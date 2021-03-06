﻿using System.Threading.Tasks;

namespace RightmoveDownloader.Services
{
	public interface IDistanceCalculationService
	{
		Task FindDistances(string toLocation);
	}
}