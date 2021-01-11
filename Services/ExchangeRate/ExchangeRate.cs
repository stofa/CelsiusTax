using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CelsiusTax.Models.ExchangeRate;
using Newtonsoft.Json;
using RestSharp;

namespace CelsiusTax.Services.ExchangeRate
{
    public interface IExchangeRateService
    {
        IEnumerable<string> GetAvailableFiatCurrencies();
        IEnumerable<HistoricalExchangeRate> GetHistoricalRates(string fiatSymbol, int year);
        decimal GetExchangeRateToUsd(string fiatSymbol);
        decimal GetExchangeRateToUsd(string fiatSymbol, DateTime date);
    }

    public class ExchangeRateService : IExchangeRateService
    {
        private static readonly object _cacheLockAvailableCurrencies = new object();
        private static IEnumerable<string> _availableFiatCurrencies;

        private static readonly object _cacheLockHistoricalrates = new object();
        private static List<HistoricalExchangeRate> _cacheHistoricalRates;

        public IEnumerable<string> GetAvailableFiatCurrencies()
        {
            LoadCache();

            return _availableFiatCurrencies;
        }

        public decimal GetExchangeRateToUsd(string fiatSymbol)
        {
            IRestResponse response = GetResponseFromApi(String.Format(Constants.ExchangeRateLatestToUsd, fiatSymbol));
            dynamic rates = JsonConvert.DeserializeObject(response.Content);

            return Convert.ToDecimal(rates["rates"][fiatSymbol].Value);
        }
        public decimal GetExchangeRateToUsd(string fiatSymbol, DateTime date)
        {
            IEnumerable<HistoricalExchangeRate> ratesForSpecificYear = GetHistoricalRates(fiatSymbol, date.Year);
            DateTime currentDate = date;

            return GetRateForSpecificDate(ratesForSpecificYear, currentDate).ExchangeRate;
        }

        private HistoricalExchangeRate GetRateForSpecificDate(IEnumerable<HistoricalExchangeRate> ratesForSpecificYear, DateTime currentDate)
        {
            HistoricalExchangeRate rateForSpecificDate = ratesForSpecificYear.SingleOrDefault(d => d.Date == currentDate.Date);

            if (rateForSpecificDate == null && currentDate.AddDays(-1).Year == currentDate.Year)
            {
                return GetRateForSpecificDate(ratesForSpecificYear, currentDate.AddDays(-1));
            }
            else if (rateForSpecificDate == null && currentDate.AddDays(-1).Year != currentDate.Year)
            {
                return GetRateForSpecificDate(ratesForSpecificYear, ratesForSpecificYear.Min(d => d.Date));
            }

            return rateForSpecificDate;
        }

        public IEnumerable<HistoricalExchangeRate> GetHistoricalRates(string fiatSymbol, int year)
        {
            LoadHistoricalRatesCache(fiatSymbol, year);

            return _cacheHistoricalRates.Where(ch => ch.FiatSymbol == fiatSymbol && ch.Date.Year == year);
        }

        private void LoadHistoricalRatesCache(string fiatSymbol, int year)
        {
            lock (_cacheLockHistoricalrates)
            {
                if (_cacheHistoricalRates == null)
                    _cacheHistoricalRates = new List<HistoricalExchangeRate>();

                if (_cacheHistoricalRates.Any(c => c.Date.Year == year && c.FiatSymbol == fiatSymbol))
                {
                    return;
                }
            }

            IRestResponse response = GetResponseFromApi(String.Format(Constants.ExchangeHistoryYearToUsd, year, fiatSymbol));
            List<HistoricalExchangeRate> result = new List<HistoricalExchangeRate>();
            dynamic rates = JsonConvert.DeserializeObject(response.Content);

            lock (_cacheLockHistoricalrates)
            {
                foreach (var rate in rates["rates"])
                {
                    _cacheHistoricalRates.Add(new HistoricalExchangeRate() { Date = Convert.ToDateTime(rate.Name), ExchangeRate = rate.Value[fiatSymbol], FiatSymbol = fiatSymbol });
                }
            }
        }

        private void LoadCache()
        {
            IRestResponse response = GetResponseFromApi(Constants.ExchangeRateAvailableCurrencies);
            dynamic rates = JsonConvert.DeserializeObject(response.Content);
            List<string> availableCurriences = new List<string>();

            foreach (var rate in rates["rates"])
            {
                availableCurriences.Add(rate.Name);
            }

            lock (_cacheLockAvailableCurrencies)
            {
                _availableFiatCurrencies = availableCurriences.OrderBy(k => k);
            }
        }

        private static IRestResponse GetResponseFromApi(string url)
        {
            var client = new RestClient(url) { Timeout = -1 };
            var request = new RestRequest(Method.GET);

            return client.Execute(request);
        }


    }
}
