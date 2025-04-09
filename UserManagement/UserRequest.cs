namespace UserManagement
{
    public class UserRequest
    {
        public class User
        {
            public Guid Id { get; set; }
            public string Auth0Id { get; set; }
            public string Username { get; set; }
            public string DisplayName { get; set; }
            public string? Bio { get; set; }
            public string? ProfilePictureUrl { get; set; }
            public string Email { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

    }
}
