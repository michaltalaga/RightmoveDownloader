using Microsoft.Extensions.Logging;
using RightmoveDownloader.Repositories;
using System.Threading.Tasks;

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
            var locations = await propertyRepository.GetLocations(false);
            logger.LogInformation($"FindPostCodes() - DONE");
        }
    }
}
