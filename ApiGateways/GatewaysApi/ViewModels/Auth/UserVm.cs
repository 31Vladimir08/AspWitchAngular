namespace GatewaysApi.ViewModels.Auth
{
    public class UserVm
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string? UserName { get; set; }
        public string? RoleCode { get; set; }
        public string? Token { get; set; }
    }
}
