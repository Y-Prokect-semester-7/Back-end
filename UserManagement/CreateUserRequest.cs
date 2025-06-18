namespace UserManagement
{
    public class CreateUserRequest
    {
        public string Auth0Id { get; set; }
        public string Username { get; set; }
        public string? Bio { get; set; }
        public string? Email { get; set; }
    }
}