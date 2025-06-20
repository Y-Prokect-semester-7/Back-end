﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement
{
    public class UserRequest
    {
        public string Auth0Id { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string Bio { get; set; } = default!;
        public string ProfilePictureUrl { get; set; } = default!;
        public string Email { get; set; } = default!;

    }
}
