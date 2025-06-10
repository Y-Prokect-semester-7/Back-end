namespace UserManagement
{
    public class CreateUserRequest
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string? Bio { get; set; }
    }
}
