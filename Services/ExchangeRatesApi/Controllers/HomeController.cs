using ExchangeRatesApi.Options;
using ExchangeRatesApi.ViewModels;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ExchangeRatesApi.Controllers
{
    [Route("/api")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ExchangeRatesOption _exchangeRatesOption;

        public HomeController(IHttpClientFactory httpClientFactory, IOptions<ExchangeRatesOption> option)
        {
            _httpClientFactory = httpClientFactory;
            _exchangeRatesOption = option.Value;
        }

        [HttpGet()]
        public async Task<IActionResult> Index()
        {
            using var client = _httpClientFactory.CreateClient();
            var stream = await client.GetStreamAsync(_exchangeRatesOption.InfoUrl);
            using var streamReader = new StreamReader(stream);
            var htmlReader = await streamReader.ReadToEndAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlReader);
            var result = new ExchangeRatesResponsetVm();
            foreach (var key in _exchangeRatesOption.Rates)
            {
                var titleNode = htmlDoc.DocumentNode.SelectSingleNode(key.Value)?.InnerText;
                result.Rates.Add(key.Key, ConverterRates(titleNode));
            }

            return Ok(result);
        }

        private decimal? ConverterRates(string? currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                return null;
            var result = Convert.ToDecimal(currency.Replace(',', '.'));
            return result;
        }
    }
}
