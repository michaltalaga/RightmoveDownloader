using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RightmoveDownloader.Clients
{
	public class GooglePlacesApiClient : IGooglePlacesApiClient
	{
		private readonly string apiKey;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<GooglePlacesApiClient> logger;

		public GooglePlacesApiClient(string apiKey, IHttpClientFactory httpClientFactory, ILogger<GooglePlacesApiClient> logger)
		{
			this.apiKey = apiKey;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
		}
		public async Task<object> FindNearestPlace(string location, string name)
		{
			logger.LogInformation($"FindNearestPlace({location},{name})");
            var url = $"https://maps.googleapis.com/maps/api/place/findplacefromtext/json?input={name}&inputtype=textquery&fields=name,geometry&locationbias=circle:5000@{location}&key=" + apiKey;
            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync(url);
            var result = await response.Content.ReadAsAsync<GooglePlacesResult>();
            var placeLocation = result.candidates?[0].geometry.location;
            if (placeLocation == null) return int.MaxValue;
            var locationLatLong = location.Split(',').Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray();
            var meters = Geolocation.GeoCalculator.GetDistance(locationLatLong[0], locationLatLong[1], placeLocation.lat, placeLocation.lng, 0, Geolocation.DistanceUnit.Meters);
            logger.LogInformation($"FindNearestPlace({location},{name}) - {meters}");
            return meters;
		}

		class GooglePlacesResult
		{
			public Candidate[] candidates { get; set; }
			public string status { get; set; }
		}

		class Candidate
		{
			public Geometry geometry { get; set; }
			public string name { get; set; }
		}

		class Geometry
		{
			public Location location { get; set; }
			public Viewport viewport { get; set; }
		}

		class Location
		{
			public float lat { get; set; }
			public float lng { get; set; }
		}

		class Viewport
		{
			public Northeast northeast { get; set; }
			public Southwest southwest { get; set; }
		}

		class Northeast
		{
			public double lat { get; set; }
			public double lng { get; set; }
		}

		class Southwest
		{
			public double lat { get; set; }
			public double lng { get; set; }
		}
	}
}