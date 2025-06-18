// Services/BlobUploadService.cs
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using TweetManagement.Repositories;

public class BlobUploadService : IBlobUploadService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName = "images";

    public BlobUploadService(IConfiguration config)
    {
        _blobServiceClient = new BlobServiceClient(config["BlobStorage:ConnectionString"]);
    }

    public async Task<string> UploadImageAndGetSasUrlAsync(IFormFile image)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(image.FileName));

        await using var stream = image.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);

        if (!blobClient.CanGenerateSasUri)
            throw new Exception("Kan SAS-token niet genereren");

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerClient.Name,
            BlobName = blobClient.Name,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        return blobClient.GenerateSasUri(sasBuilder).ToString();
    }

    public async Task DeleteImageAsync(string blobUrl)
    {
        var blobClient = new BlobClient(new Uri(blobUrl));
        await blobClient.DeleteIfExistsAsync();
    }

}
