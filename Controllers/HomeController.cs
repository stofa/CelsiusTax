using System;
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

        public IActionResult Index(string apiKey, DateTime? from, DateTime? to, string fiatCurrency)
        {
            if(!from.HasValue && !to.HasValue)
            {
                from = new DateTime(DateTime.Now.Year - 1, 1, 1);
                to = new DateTime(DateTime.Now.Year - 1, 12, 31);
            }

            TaxReportViewModel model = new TaxReportViewModel
            {
                AvailableCurrencies = _exchangeRateService.GetAvailableFiatCurrencies().OrderBy(c => c),
                SelectedCurrency = GetFiatCurrency(fiatCurrency),
                From = from.Value,
                To = to.Value
            };

            if (!string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(fiatCurrency))
            {
                try
                {
                    model.InterestsPerCoin = _transactionService.GetInterestsForSpecificTimeframe(apiKey, from.Value, to.Value, fiatCurrency);
                }
                catch (Exception ex)
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
