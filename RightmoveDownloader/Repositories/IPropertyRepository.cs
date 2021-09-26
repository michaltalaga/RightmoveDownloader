using RightmoveDownloader.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RightmoveDownloader.Repositories
{
    public interface IPropertyRepository
    {
        Task AddProperties(IEnumerable<Property> properties);
        Task<IEnumerable<string>> GetLocations(bool includeCalculated = false);
        Task AddTravelTimes(IEnumerable<IGoogleMapsDistanceApiClient.TravelInfo> travelTimes);

        public class Property
        {
            public string Id { get; init; }
            public string PriceFrequency { get; init; }
            public int PriceAmount { get; init; }
            public int Bedrooms { get; init; }
            public int NumberOfFloorplans { get; init; }
            public string PropertyUrl { get; init; }
            public double Longitude { get; init; }
            public double Latitude { get; init; }
        }
        public class LocationPostCode
        {
            public string PostCode { get; set; }
            public string Location { get; set; }
        }
    }
}