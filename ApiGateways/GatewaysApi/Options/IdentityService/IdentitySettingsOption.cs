namespace GatewaysApi.Options.IdentityService
{
    public class IdentitySettingsOption
    {
        public string? Name { get; init; }
        public string? IdentityServiceApi { get; init; }
        public string? ClientId { get; init; }
        public string? ClientSecret { get; init; }
        public string? Scope { get; init; }
    }
}
