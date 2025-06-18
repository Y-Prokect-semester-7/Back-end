using System.ComponentModel.DataAnnotations;

namespace TweetManagement.Models
{
    public class TweetRequest
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Content { get; set; }

        public string? MediaUrl { get; set; }

        [Required]
        public bool Visibility { get; set; }

        public IFormFile? Image { get; set; }
    }
}
