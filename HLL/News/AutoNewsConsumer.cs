using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using LazyCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using House.HLL.News.Interfaces;
using House.HLL.News.Models;

namespace House.HLL.News
{
    public class AutoNewsConsumer : IAutoNewsConsumer
    {
        private readonly IRestClient _newsClient;
        private readonly IAppCache _cache;
        public AutoNewsConsumer(IOptions<NewsApi> options, IOptions<ConnectionStrings> connectionStrings, IAppCache cache)
        {
            _newsClient = new RestClient($"{connectionStrings.Value.NewsApi}{options.Value.Key}");
            _cache = cache;
        }

        public List<NewsMessage> CurrentNews() => 
            GetNewsData().Articles.Select(x => new NewsMessage
            {
                Message = x.Title,
                CreatedBy = x.Source.Name
            }).ToList();

        private NewsRoot GetNewsData()
        {
            return _cache.GetOrAdd($"{GetType().FullName}_news_data", () =>
            {
                var request = new RestRequest(Method.GET);
                var result = _newsClient.Execute(request);
                return JsonConvert.DeserializeObject<NewsRoot>(result.Content);
            }, new Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions { AbsoluteExpiration = DateTime.Now.AddMinutes(5) });
            
        }
    }
}
