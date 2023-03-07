using System.ComponentModel.DataAnnotations;

namespace GatewaysApi.ViewModels.Auth
{
    public class LoginVm
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
