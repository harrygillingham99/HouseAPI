using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using House.HLL.Cat.Interfaces;
using Microsoft.Extensions.Options;
using RestSharp;

namespace House.HLL.Cat
{
    public class CatServiceClient : ICatServiceClient
    {
        private readonly RestClient _catClient;
        private readonly Random _random;
        private readonly string _getCatUrlStem;

        public CatServiceClient(IOptions<ConnectionStrings> connectionStrings)
        {
            _random = new Random();
            _catClient = new RestClient(connectionStrings.Value.CatTags);
            _getCatUrlStem = connectionStrings.Value.GetCat;
        }

        public async Task<string> GetRandomCatUrl()
        {
            var tagsRequest = new RestRequest();
            var tags = await _catClient.GetAsync<List<string>>(tagsRequest);
            var randomTag = tags[_random.Next(tags.Count)];

            return string.Format(_getCatUrlStem, randomTag);
        }
    }
}
