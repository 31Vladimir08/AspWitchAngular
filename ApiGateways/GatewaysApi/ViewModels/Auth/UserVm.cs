namespace GatewaysApi.ViewModels.Auth
{
    public class UserVm
    {
        public string UserId { get; set; }
        public string Login { get; set; }
        public string? UserName { get; set; }
        public string? Role { get; set; }
        public string Token { get; set; }
    }
}
