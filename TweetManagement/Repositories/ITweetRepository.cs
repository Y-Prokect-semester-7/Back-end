using TweetManagement.Models;

namespace TweetManagement.Repositories
{
    public interface ITweetRepository
    {
        Task AddTweetAsync(TweetRequest tweet);
    }
}
