﻿namespace GatewaysApi.ModelDto.Auth
{
    public class UserDto
    {
        public string Id { get; init; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string RoleCode { get; set; }
    }
}
