using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace CelsiusTax.Services.Celsius
{
    public interface ICelsiusApiService
    {
        IRestResponse GetResultFromCelsiusPrivateApi(string apiKey, string url);

        IRestResponse GetResultFromCelsiusPublicApi(string url);
    }

    public class CelsiusApiService : ICelsiusApiService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CelsiusApiService> _logger;

        public CelsiusApiService(IConfiguration configuration, ILogger<CelsiusApiService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IRestResponse GetResultFromCelsiusPrivateApi(string apiKey, string url)
        {
            var client = new RestClient(url) { Timeout = -1 };
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-Cel-Partner-Token", _configuration["CelsiusApi:PrivateApiKey"]);
            request.AddHeader("X-Cel-Api-Key", apiKey);

            return client.Execute(request);
        }

        public IRestResponse GetResultFromCelsiusPublicApi(string url)
        {
            var client = new RestClient(url)
            {
                Timeout = -1
            };
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-api-key", _configuration["CelsiusApi:PublicApiKey"]);
            return client.Execute(request);
        }
    }
}
