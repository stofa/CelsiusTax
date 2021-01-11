using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CelsiusTax.Services.Celsius
{
    public interface ICurrencyService
    {
        IEnumerable<string> GetSupportedCurrencies(string apiKey);
    }

    public class CurrencyService : ICurrencyService
    {
        private readonly ICelsiusApiService _celsiusApiService;
        private static readonly object CacheLock = new object();
        private static IEnumerable<string> _supportedCurrenciesCache;

        public CurrencyService(ICelsiusApiService celsiusApiService)
        {
            _celsiusApiService = celsiusApiService;
        }

        public IEnumerable<string> GetSupportedCurrencies(string apiKey)
        {
            LoadCache(apiKey);
            return _supportedCurrenciesCache;
        }

        private void LoadCache(string apiKey)
        {
            if (_supportedCurrenciesCache == null)
            {
                var tmpCurrencies = LoadSupportedCurrenciesFromCelsius(apiKey);

                lock (CacheLock)
                {
                    _supportedCurrenciesCache = tmpCurrencies;
                }
            }
        }

        private List<string> LoadSupportedCurrenciesFromCelsius(string apiKey)
        {
            var currenciesReponse =
                _celsiusApiService.GetResultFromCelsiusPrivateApi(apiKey, Constants.CelsiusApiGetSupportedCurrencies);
            dynamic currencies = JsonConvert.DeserializeObject(currenciesReponse.Content);

            List<string> tmpCurrencies = new List<string>();

            foreach (var currency in currencies["currencies"])
            {
                tmpCurrencies.Add(currency.ToString());
            }

            return tmpCurrencies;
        }
    }
}
