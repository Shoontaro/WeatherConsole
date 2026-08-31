using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;


namespace WeatherConsole
{
    public class GetWether
    {
        string apiKey;
        const string url = "https://visualcrossing.com";
        IMemoryCache cache;

        public GetWether()
        {
            apiKey = new GetApiKey().GetKey();
            cache = new MemoryCache(new MemoryCacheOptions());
        }

        public void Get()
        {
            using IMemoryCache cache1 = new MemoryCache(new MemoryCacheOptions());
            string location = "Minsk";
            string cacheKey = $"weather_{location.ToLower()}";

            var weather = cache1.GetOrCreate(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

                Console.WriteLine("[СЕТЬ] Данных в кэше нет. Делаем реальный запрос через Flurl...");

                    var response = await url
                .AppendPathSegment(location)
                .AppendPathSegment("today")
                .SetQueryParams(new
                {
                    unitGroup = "metric",
                    include = "current",
                    key = apiKey,
                    contentType = "json"
                })
                .GetJsonAsync<VisualCrossingResponse>();

                    return response;
                
            });
        }
    }
}
