using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TweetManagement.Controllers
{
    [ApiController]
    [Route("api/upload")]
    public class UploadController : ControllerBase
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "images";

        public UploadController(IConfiguration configuration)
        {
            _blobServiceClient = new BlobServiceClient(configuration["BlobStorage:ConnectionString"]);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No image provided.");

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(image.FileName));

            await using var stream = image.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            if (!blobClient.CanGenerateSasUri)
                return StatusCode(500, "SAS-token kan niet worden gegenereerd");

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerClient.Name,
                BlobName = blobClient.Name,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasUri = blobClient.GenerateSasUri(sasBuilder);

            return Ok(new { url = sasUri.ToString() });
        }
    }
}
