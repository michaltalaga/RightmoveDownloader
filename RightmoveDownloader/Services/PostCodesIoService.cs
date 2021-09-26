using Microsoft.Extensions.Logging;
using MoreLinq;
using RightmoveDownloader.Repositories;
using System.Threading.Tasks;
using System.Linq;
using System.Globalization;

namespace RightmoveDownloader.Services
{
    public class PostCodesIoService : IPostCodeService
    {
        private readonly IPropertyRepository propertyRepository;
        private readonly ILogger<PostCodesIoService> logger;

        public PostCodesIoService(IPropertyRepository propertyRepository, ILogger<PostCodesIoService> logger)
        {
            this.propertyRepository = propertyRepository;
            this.logger = logger;
        }
        public async Task FindPostCodes()
        {
            logger.LogInformation($"FindPostCodes()");
            var locations = await propertyRepository.GetLocations(true);
            var postcodesIOClient = new MarkEmbling.PostcodesIO.PostcodesIOClient();
            foreach (var locationsBatch in locations.Batch(10))
            {
                var postCodes = postcodesIOClient.BulkLookupLatLon(locationsBatch.Select(l => new MarkEmbling.PostcodesIO.ReverseGeocodeQuery { Latitude = double.Parse(l.Split(',')[0], CultureInfo.InvariantCulture), Longitude = double.Parse(l.Split(',')[1], CultureInfo.InvariantCulture)  }));
                await propertyRepository.AddLocationsPostCodes(postCodes.Select(pc => new IPropertyRepository.LocationPostCode
                {
                    Location = pc.Query.Latitude + "," + pc.Query.Longitude,
                    PostCode = pc.Result[0].Postcode
                }));
            }
            logger.LogInformation($"FindPostCodes() - DONE");
        }
    }
}
