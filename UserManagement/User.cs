using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserManagement
{
    public class User
    {
        public int Id { get; set; }

        public string Auth0Id { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Bio { get; set; } = default!;
        public string Email { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
