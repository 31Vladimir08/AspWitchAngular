namespace ExchangeRatesApi.Options
{
    public class ExchangeRatesOption
    {
        public string InfoUrl { get; init; }
        public Dictionary<string, string> Rates { get; set; }
    }
}
