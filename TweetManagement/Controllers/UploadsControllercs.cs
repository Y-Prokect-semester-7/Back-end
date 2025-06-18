using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TweetManagement.Controllers
{
    [ApiController]
    [Route("api/blobcode")]
    public class UploadsControllercs : ControllerBase
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "images";

        public UploadsControllercs(IConfiguration configuration)
        {
            _blobServiceClient = new BlobServiceClient(configuration["BlobStorage:ConnectionString"]);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetImageUrl(string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("images");
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!blobClient.CanGenerateSasUri)
                return StatusCode(500, "Cannot generate SAS");

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerClient.Name,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(15)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasUri = blobClient.GenerateSasUri(sasBuilder);
            return Ok(new { url = sasUri.ToString() });
        }
    }
}
