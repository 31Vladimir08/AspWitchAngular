using IdentityService.ModelDto;

namespace IdentityService.ViewModels
{
    public class RegistrationVm
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string RoleCode { get; set; }
    }
}
