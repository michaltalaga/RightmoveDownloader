using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using RightmoveDownloader.Clients;
using RightmoveDownloader.Repositories;
using Microsoft.Extensions.Logging;

namespace RightmoveDownloader.Services
{
    public partial class RightmoveDownloadService : IRightmoveDownloadService
    {
        private readonly IRightmoveHttpClient rightmoveHttpClient;
        private readonly IPropertyRepository propertiesRepository;
        private readonly ILogger<RightmoveDownloadService> logger;

        public RightmoveDownloadService(IRightmoveHttpClient rightmoveHttpClient, IPropertyRepository propertiesRepository, ILogger<RightmoveDownloadService> logger)
        {
            this.rightmoveHttpClient = rightmoveHttpClient;
            this.propertiesRepository = propertiesRepository;
            this.logger = logger;
        }
        public async Task Download(string locationIdentifier, int radius, int minBedrooms, int maxBedrooms, int minPrice, int maxPrice, string channel)
        {
            logger.LogInformation($"Download({locationIdentifier}, {radius}, {minBedrooms}, {maxBedrooms}, {minPrice}, {maxPrice})");
            var propertyBatches = rightmoveHttpClient.GetProperties(locationIdentifier, radius, minBedrooms, maxBedrooms, minPrice, maxPrice, channel);
            await foreach (var properties in propertyBatches)
            {
                var props = properties.Select(p => new IPropertyRepository.Property
                {
                    Id = p.id,
                    Bedrooms = p.bedrooms,
                    NumberOfFloorplans = p.numberOfFloorplans,
                    PriceAmount = p.price.amount,
                    PriceFrequency = p.price.frequency,
                    PriceQualifier = p.price.displayPrices?[0]?.displayPriceQualifier,
                    PropertyType = p.propertySubType,
                    PropertyUrl = p.propertyUrl,
                    Longitude = p.location.longitude,
                    Latitude = p.location.latitude
                });
                await propertiesRepository.AddProperties(props);
            }
            logger.LogInformation($"Download({locationIdentifier}, {radius}, {minBedrooms}, {maxBedrooms}, {minPrice}, {maxPrice}) - DONE");
        }
    }
}