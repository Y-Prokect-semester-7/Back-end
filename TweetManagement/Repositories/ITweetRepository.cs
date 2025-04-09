using TweetManagement.Models;

namespace TweetManagement.Repositories
{
    public interface ITweetRepository
    {
        Task AddTweetAsync(TweetRequest tweet);
        Task<List<TweetModel>> GetTweetsAsync(string userId);
    }
}
