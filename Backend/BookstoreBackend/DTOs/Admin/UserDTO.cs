﻿namespace BookstoreBackend.DTOs.Admin
{
    public class UserDTO
    {
        public required string Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required IList<string> Roles { get; set; }
    }
}
