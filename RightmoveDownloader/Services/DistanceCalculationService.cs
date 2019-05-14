using RightmoveDownloader.Clients;
using RightmoveDownloader.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RightmoveDownloader.Services
{
	public class DistanceCalculationService : IDistanceCalculationService
	{
		private readonly IGoogleSheetsClient googleSheetsClient;
		private readonly IPropertyRepository propertyRepository;

		public DistanceCalculationService(IPropertyRepository propertyRepository)
		{
			this.propertyRepository = propertyRepository;
		}
		public async Task Process()
		{
			var locations = propertyRepository.GetLocations(false).ToList();
		}
	}
}
