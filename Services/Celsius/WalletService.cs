using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CelsiusTax.Models.Wallet;
using CelsiusTax.Services.ExchangeRate;
using Newtonsoft.Json;

namespace CelsiusTax.Services.Celsius
{
    public interface IWalletService
    {
        WalletModel GetWallet(string apiKey, string fiatCurrencyToDisplayValues);
    }

    public class WalletService : IWalletService
    {
        private const string UsdFiatCurrencySymbol = "USD";
        private readonly ICelsiusApiService _celsiusApiService;
        private readonly ICurrencyService _currencyService;
        private readonly IExchangeRateService _exchangeService;

        public WalletService(ICelsiusApiService celsiusApiService, ICurrencyService currencyService
            , IExchangeRateService exchangeService)
        {
            _celsiusApiService = celsiusApiService;
            _currencyService = currencyService;
            _exchangeService = exchangeService;
        }

        public WalletModel GetWallet(string apiKey, string fiatCurrencyToDisplayValues)
        {
            WalletModel model = new WalletModel();
            List<BalanceModel> balances = new List<BalanceModel>();

            var response = _celsiusApiService.GetResultFromCelsiusPrivateApi(apiKey, Constants.CelsiusApiGetWalletBalance);
            dynamic des = JsonConvert.DeserializeObject(response.Content);

            decimal fiatExchangeRateToUsd = 1;

            if (fiatCurrencyToDisplayValues.ToUpper() != UsdFiatCurrencySymbol)
            {
                fiatExchangeRateToUsd = _exchangeService.GetExchangeRateToUsd(fiatCurrencyToDisplayValues);
            }

            foreach (var currency in _currencyService.GetSupportedCurrencies(apiKey))
            {
                string currentCurrency = currency.ToString();
                decimal amount = des["balance"][currentCurrency.ToLower()];

                if (amount > 0)
                {
                    var balanceModel = new BalanceModel
                    {
                        Amount = amount,
                        Currency = currency,
                        ValueInUsd = GetValueInUsd(apiKey, currentCurrency),
                        FiatCurrencyToDisplay = fiatCurrencyToDisplayValues
                    };

                    balanceModel.ValueInCurrencyToDisplay = balanceModel.ValueInUsd * fiatExchangeRateToUsd;
                    balances.Add(balanceModel);
                }
            }

            model.Balances = balances.OrderByDescending(b => b.ValueInUsd);
            model.CurrentCelBalance = balances.SingleOrDefault(b => b.Currency == Constants.CelTicker)?.Amount;
            return model;
        }

        private decimal GetValueInUsd(string apiKey, string currentCurrency)
        {
            var balancePerCoinResponse =
                _celsiusApiService.GetResultFromCelsiusPrivateApi(apiKey,
                    String.Format(Constants.CelsiusApiGetCoinBalance, currentCurrency));
            dynamic balancePerCoin = JsonConvert.DeserializeObject(balancePerCoinResponse.Content);
            return Convert.ToDecimal(balancePerCoin["amount_in_usd"]);
        }
    }
}
