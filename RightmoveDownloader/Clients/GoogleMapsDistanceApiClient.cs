﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RightmoveDownloader.Clients
{
	public class GoogleMapsDistanceApiClient : IGoogleMapsDistanceApiClient
	{
		public int GetMinutesBetweenPoints(string fromLocation, string toLocation)
		{
			return 0;
		}
		class GoogleapisResult
		{
			public Geocoded_Waypoints[] geocoded_waypoints { get; set; }
			public Route[] routes { get; set; }
			public string status { get; set; }
		}

		class Geocoded_Waypoints
		{
			public string geocoder_status { get; set; }
			public string place_id { get; set; }
			public string[] types { get; set; }
		}

		class Route
		{
			public Bounds bounds { get; set; }
			public string copyrights { get; set; }
			public Leg[] legs { get; set; }
			public Overview_Polyline overview_polyline { get; set; }
			public string summary { get; set; }
			public string[] warnings { get; set; }
			public object[] waypoint_order { get; set; }
		}

		class Bounds
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

		class Overview_Polyline
		{
			public string points { get; set; }
		}

		class Leg
		{
			public Arrival_Time arrival_time { get; set; }
			public Departure_Time departure_time { get; set; }
			public Distance distance { get; set; }
			public Duration duration { get; set; }
			public string end_address { get; set; }
			public End_Location end_location { get; set; }
			public string start_address { get; set; }
			public Start_Location start_location { get; set; }
			public Step[] steps { get; set; }
			public object[] traffic_speed_entry { get; set; }
			public object[] via_waypoint { get; set; }
		}

		class Arrival_Time
		{
			public string text { get; set; }
			public string time_zone { get; set; }
			public int value { get; set; }
		}

		class Departure_Time
		{
			public string text { get; set; }
			public string time_zone { get; set; }
			public int value { get; set; }
		}

		class Distance
		{
			public string text { get; set; }
			public int value { get; set; }
		}

		class Duration
		{
			public string text { get; set; }
			public int value { get; set; }
		}

		class End_Location
		{
			public float lat { get; set; }
			public float lng { get; set; }
		}

		class Start_Location
		{
			public float lat { get; set; }
			public float lng { get; set; }
		}

		class Step
		{
			public Distance1 distance { get; set; }
			public Duration1 duration { get; set; }
			public End_Location1 end_location { get; set; }
			public string html_instructions { get; set; }
			public Polyline polyline { get; set; }
			public Start_Location1 start_location { get; set; }
			public Step1[] steps { get; set; }
			public string travel_mode { get; set; }
			public Transit_Details transit_details { get; set; }
		}

		class Distance1
		{
			public string text { get; set; }
			public int value { get; set; }
		}

		class Duration1
		{
			public string text { get; set; }
			public int value { get; set; }
		}

		class End_Location1
		{
			public float lat { get; set; }
			public float lng { get; set; }
		}

		class Polyline
		{
			public string points { get; set; }
		}

		class Start_Location1
		{
			public float lat { get; set; }
			public float lng { get; set; }
		}

		class Transit_Details
		{
			public Arrival_Stop arrival_stop { get; set; }
			public Arrival_Time1 arrival_time { get; set; }
			public Departure_Stop departure_stop { get; set; }
			public Departure_Time1 departure_time { get; set; }
			public string headsign { get; set; }
			public Line line { get; set; }
			public int num_stops { get; set; }
		}

		class Arrival_Stop
		{
			public Location location { get; set; }
			public string name { get; set; }
		}

		class Location
		{
			public float lat { get; set; }
			public float lng { get; set; }
		}

		class Arrival_Time1
		{
			public string text { get; set; }
			public string time_zone { get; set; }
			public int value { get; set; }
		}

		class Departure_Stop
		{
			public Location1 location { get; set; }
			public string name { get; set; }
		}

		class Location1
		{
			public float lat { get; set; }
			public float lng { get; set; }
		}

		class Departure_Time1
		{
			public string text { get; set; }
			public string time_zone { get; set; }
			public int value { get; set; }
		}

		class Line
		{
			public Agency[] agencies { get; set; }
			public string color { get; set; }
			public string name { get; set; }
			public string short_name { get; set; }
			public string text_color { get; set; }
			public Vehicle vehicle { get; set; }
		}

		class Vehicle
		{
			public string icon { get; set; }
			public string local_icon { get; set; }
			public string name { get; set; }
			public string type { get; set; }
		}

		class Agency
		{
			public string name { get; set; }
			public string phone { get; set; }
			public string url { get; set; }
		}

		class Step1
		{
			public Distance2 distance { get; set; }
			public Duration2 duration { get; set; }
			public End_Location2 end_location { get; set; }
			public string html_instructions { get; set; }
			public Polyline1 polyline { get; set; }
			public Start_Location2 start_location { get; set; }
			public string travel_mode { get; set; }
			public string maneuver { get; set; }
		}

		class Distance2
		{
			public string text { get; set; }
			public int value { get; set; }
		}

		class Duration2
		{
			public string text { get; set; }
			public int value { get; set; }
		}

		class End_Location2
		{
			public float lat { get; set; }
			public float lng { get; set; }
		}

		class Polyline1
		{
			public string points { get; set; }
		}

		class Start_Location2
		{
			public float lat { get; set; }
			public float lng { get; set; }
		}

	}
}