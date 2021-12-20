using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using House.HLL.Dashboard.WeatherFeed.Interfaces;
using House.HLL.Dashboard.WeatherFeed.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using Serilog;

namespace House.HLL.Dashboard.WeatherFeed.ServiceAgents
{
    public class WeatherServiceAgent : IWeatherServiceAgent
    {
        private readonly string _apiKey;
        private readonly IRestClient _weatherClient;

        public WeatherServiceAgent(IOptions<OpenWeatherApi> openWeatherApi, IOptions<ConnectionStrings> connectionStrings)
        {
            _apiKey = openWeatherApi.Value.Key;
            _weatherClient = new RestClient(connectionStrings.Value.OpenWeather);
        }

        public Task<OpenWeatherCurrent> Get()
        {
            var request = new RestRequest(Method.GET)
                .AddParameter("q", "Bournemouth")
                .AddParameter("units", "metric") // important, default is Kelvin
                .AddParameter("appid", _apiKey);
            return Retry.Retry.DoAsync(() => GetWeatherData(request), TimeSpan.FromSeconds(1));

        }

        private async Task<OpenWeatherCurrent> GetWeatherData(IRestRequest request)
        {
            var result = await _weatherClient.ExecuteAsync<OpenWeatherCurrent>(request);
            
            if (result == null)
                throw new NullReferenceException();

            if (result.StatusCode == HttpStatusCode.OK) return result.Data;

            var msg = $"Unexpected error {(int)result.StatusCode} status code from result";
            Log.Error(msg);
            throw new HttpRequestException(msg);
        }
    }
}
