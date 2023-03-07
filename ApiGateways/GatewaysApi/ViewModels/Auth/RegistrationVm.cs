using System.ComponentModel.DataAnnotations;

namespace GatewaysApi.ViewModels.Auth
{
    public class RegistrationVm
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
