using Azure.Identity;
using Azure.Storage.Blobs;
using Backend.ImageUploadModule;

namespace Backend.Implementations
{
    public class ImageBlobStorageService : IImageStorageService
    {
        public IConfiguration Configuration { get; set; }

        public ImageBlobStorageService(IConfiguration config)
        {
            Configuration = config;
        }

        public Task<string> UploadImageAsync(Guid imageId, Stream imageStream)
        {
            var serviceClient = new BlobServiceClient(Configuration.GetConnectionString("blobConnect"));
            var containerClient = serviceClient.GetBlobContainerClient("abctak-container");
            var blob = containerClient.GetBlobClient(imageId.ToString());

            blob.Upload(imageStream);

            return Task.FromResult(blob.Uri.ToString());
        }
    }
}
