namespace ExchangeRatesApi.ViewModels
{
    public class ExchangeRatesResponsetVm
    {
        public ExchangeRatesResponsetVm()
        {
            Rates = new Dictionary<string, decimal?>();
        }

        public Dictionary<string, decimal?> Rates { get; init; }
    }
}
