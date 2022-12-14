using CelsiusTax.Models.ExchangeRate;
using CelsiusTax.Models.Interests;
using CelsiusTax.Models.Transaction;
using CelsiusTax.Services.ExchangeRate;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CelsiusTax.Services.Celsius
{

    public interface ITransactionService
    {
        List<CelsiusTransactionModel> GetTransactionsForSpecificTimeframe(string apiKey, DateTime from, DateTime to);
        IEnumerable<InterestsPerCoin> GetInterestsForSpecificTimeframe(string apiKey, DateTime from, DateTime to, string fiatCurrencyToCalcValue);
    }

    public class TransactionService : ITransactionService
    {
        private const int _pageSizeGetTransaction = 400;
        private const string CelsiusTransactionNatureInterests = "interest";
        private const string CelsiusTransactionStateConfirmed = "confirmed";

        private readonly ICelsiusApiService _celsiusApiService;
        private readonly IExchangeRateService _rateService;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(ICelsiusApiService celsiusApiService, IExchangeRateService rateService, ILogger<TransactionService> logger)
        {
            _celsiusApiService = celsiusApiService;
            _rateService = rateService;
            _logger = logger;
        }

        public IEnumerable<InterestsPerCoin> GetInterestsForSpecificTimeframe(string apiKey, DateTime from, DateTime to, string fiatCurrencyToCalcValue)
        {
            IEnumerable<CelsiusTransactionModel> transactionsPerYear = GetTransactionsForSpecificTimeframe(apiKey, from, to)
                .Where(t => t.nature == CelsiusTransactionNatureInterests && t.state == CelsiusTransactionStateConfirmed);

            List<InterestsPerCoin> interestsByCoin = new List<InterestsPerCoin>();

            foreach (var interestByCoin in transactionsPerYear.GroupBy(t => t.original_interest_coin))
            {
                interestsByCoin.Add(new InterestsPerCoin()
                {
                    Coin = interestByCoin.Key,
                    UsdValue = interestByCoin.Sum(c => c.amount_usd),
                    ValueInSelectedFiat = interestByCoin.Sum(c => _rateService.GetExchangeRateToUsd(fiatCurrencyToCalcValue, c.time.Date) * c.amount_usd)
                });
            }

            return interestsByCoin.Where(i => i.UsdValue > new decimal(0.01));
        }

        public List<CelsiusTransactionModel> GetTransactionsForSpecificTimeframe(string apiKey, DateTime from, DateTime to)
        {
            List<CelsiusTransactionModel> transactionInSpecificTimeFrame = new List<CelsiusTransactionModel>();
            CelsiusGetTransactionResult getTransactionResult = GetResults(apiKey, 1);

            transactionInSpecificTimeFrame.AddRange(getTransactionResult.record.Where(r => r.time >= from && r.time <= to));

            for (int i = 2; i < getTransactionResult.pagination.pages + 1; i++)
            {
                transactionInSpecificTimeFrame.AddRange(GetResults(apiKey, i).record.Where(r => r.time >= from && r.time <= to));
            }

            return transactionInSpecificTimeFrame;
        }

        private CelsiusGetTransactionResult GetResults(string apiKey, int page)
        {
            var result = _celsiusApiService.GetResultFromCelsiusPrivateApi(apiKey, $"{Constants.CelsiusApiGetWalletTransactions}?per_page={_pageSizeGetTransaction}&page={page}");
            return (CelsiusGetTransactionResult)JsonConvert.DeserializeObject(result.Content, typeof(CelsiusGetTransactionResult));
        }
    }
}
