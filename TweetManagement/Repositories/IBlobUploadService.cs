namespace TweetManagement.Repositories
{
    public interface IBlobUploadService
    {
        Task<string> UploadImageAndGetSasUrlAsync(IFormFile image);

        Task DeleteImageAsync(string blobUrl);
    }
}
