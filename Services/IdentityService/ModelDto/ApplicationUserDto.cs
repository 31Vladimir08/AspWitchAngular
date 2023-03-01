namespace IdentityService.ModelDto
{
    public class ApplicationUserDto
    {
        public string Id { get; init; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
    }
}
