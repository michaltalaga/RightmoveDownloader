using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RightmoveDownloader.Clients
{
	public class GooglePlacesApiClient : IGooglePlacesApiClient
	{
		private readonly string apiKey;
		private readonly ILogger<GooglePlacesApiClient> logger;

		public GooglePlacesApiClient(string apiKey, ILogger<GooglePlacesApiClient> logger)
		{
			this.apiKey = apiKey;
			this.logger = logger;
		}
		public async Task<object> FindNearestPlace(string location, string name)
		{
			logger.LogInformation($"FindNearestPlace({location},{name})");
			//https://maps.googleapis.com/maps/api/place/findplacefromtext/json?input=primark&inputtype=textquery&fields=name,geometry&locationbias=circle:2000@51.3848465,-0.0848496&key=
			return null;
			logger.LogInformation($"FindNearestPlace({location},{name}) - DONE");
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
			public float lat { get; set; }
			public float lng { get; set; }
		}

		class Southwest
		{
			public float lat { get; set; }
			public float lng { get; set; }
		}
	}
}