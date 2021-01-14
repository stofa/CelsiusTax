﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CelsiusTax.Models;
using CelsiusTax.Services.Celsius;
using CelsiusTax.Models.Interests;
using CelsiusTax.Services.ExchangeRate;

namespace CelsiusTax.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITransactionService _transactionService;
        private readonly IExchangeRateService _exchangeRateService;

        public HomeController(ILogger<HomeController> logger, ITransactionService transactionService,
            IExchangeRateService exchangeRateService)
        {
            _logger = logger;
            _transactionService = transactionService;
            _exchangeRateService = exchangeRateService;
        }

        public IActionResult Index(string apiKey, int taxYear, string fiatCurrency)
        {
            TaxReportViewModel model = new TaxReportViewModel
            {
                AvailableCurrencies = _exchangeRateService.GetAvailableFiatCurrencies().OrderBy(c => c),
                SelectedCurrency = GetFiatCurrency(fiatCurrency),
                TaxYear = taxYear
            };

            if (!string.IsNullOrEmpty(apiKey))
            {
                try
                {
                    model.InterestsPerCoin = _transactionService.GetInterestsForSpecificYear(apiKey, taxYear, fiatCurrency);
                }
                catch (Exception)
                {
                    ViewBag.ExceptionOcurred = true;
                    return View(model);
                }
            }

            return View(model);
        }

        private static string GetFiatCurrency(string fiatCurrency)
        {
            if (string.IsNullOrEmpty(fiatCurrency))
            {
                fiatCurrency = Constants.UsdCurrency;

                var currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;

                if (currentCulture.Name.ToLower().Contains(Constants.ChCulture))
                    fiatCurrency = Constants.ChfCurrency;
                else
                {
                    switch (currentCulture.TwoLetterISOLanguageName.ToLower())
                    {
                        case Constants.DeCulture:
                        case Constants.FrCulture:
                            fiatCurrency = Constants.EurCurrency;
                            break;
                        default:
                            break;
                    }
                }

            }
            return fiatCurrency;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
