﻿//using RightmoveDownloader.Clients;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace RightmoveDownloader.Repositories
//{
//	public class InMemoryPropertyRepository : IPropertyRepository
//	{
//		ConcurrentDictionary<string, RightmoveHttpClient.Property> properties = new ConcurrentDictionary<string, RightmoveHttpClient.Property>();
//		public void AddProperties(IEnumerable<RightmoveHttpClient.Property> properties)
//		{
//			foreach (var property in properties)
//			{
//				this.properties[property.id] = property;
//			}
//		}
//		public IEnumerable<string> GetLocations(bool includeCalculated = false)
//		{
//			return properties.Select(x => x.Value.location.latitude + "," + x.Value.location.longitude).Distinct();
//		}
//	}
//}