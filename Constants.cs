using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CelsiusTax
{
    public static class Constants
    {
        public const string CelsiusApiRootUrl = "https://wallet-api.celsius.network";
        public static string CelsiusApiGetSupportedCurrencies => $"{CelsiusApiRootUrl}/util/supported_currencies";
        public static string CelsiusApiGetWalletBalance => $"{CelsiusApiRootUrl}/wallet/balance";
        public static string CelsiusApiGetWalletTransactions => $"{CelsiusApiRootUrl}/wallet/transactions";
        public static string CelsiusApiGetCoinBalance => $"{CelsiusApiRootUrl}/wallet/{{0}}/balance";

        public const string CelsiusPublicApiRootUrl = "https://celsius.network/api";
        public static string CelsiusTop100Url => $"{CelsiusPublicApiRootUrl}/community/top100";


        public const string ExchangeRateApiRootUrl = "https://api.exchangerate.host";
        public static string ExchangeRateAvailableCurrencies => $"{ExchangeRateApiRootUrl}/latest?base=USD";

        public static string ExchangeRateLatestToUsd => $"{ExchangeRateApiRootUrl}/latest?base=USD&symbols={{0}}";
        public static string ExchangeHistoryYearToUsd => $"{ExchangeRateApiRootUrl}/timeseries?start_date={{0}}-01-01&end_date={{0}}-12-31&base=USD&symbols={{1}}";

        public const string CelTicker = "CEL";
        public const string UsdCurrency = "USD";
        public const string ChfCurrency = "CHF";
        public const string EurCurrency = "EUR";
        public const string ChCulture = "-ch";
        public const string DeCulture = "de";
        public const string FrCulture = "fr";
    }
}
