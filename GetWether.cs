using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http.Json;
using System.Text;


namespace WeatherConsole
{
    public class GetWether
    {
        string apiKey;
        const string url = "https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline";
        
        IView view;

        private static readonly IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());


        public GetWether(IView view)
        {
            apiKey = new GetApiKey().GetKey();
            this.view = view;
        }

        public async Task GetWeatherAsync()
        {
            view.Write("Town: ");
            string location = Console.ReadLine()??"Minsk";
            string cacheKey = $"weather_{location.ToLower()}";

            var weather = await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

                view.WriteLine("[NETWORK] No data in cache. Making a real request via Flurl...");

                try
                {
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

                }
                catch(FlurlHttpException ex)
                {
                    var status = ex.StatusCode;
                    view.WriteLine($"[ERROR] Network error or invalid request. Status code: {status}");

                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.Zero;
                    return null;
                }
                catch (Exception ex)
                {
                    view.WriteLine($"[ERROR] Unexpected error: {ex.Message}");
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.Zero;
                    return null;
                }
            });

            if (weather?.CurrentConditions != null)
            {
                var current = weather.CurrentConditions;
                view.WriteLine($"\n[INFO]  Town : {weather.Address}");
                view.WriteLine($"[INFO]-> Температура: {current.Temp} °C (Ощущается как: {current.FeelsLike} °C)");
                view.WriteLine($"[INFO]-> Погода: {current.Conditions}");
                view.WriteLine($"[INFO]-> Влажность: {current.Humidity}%");
                view.WriteLine($"[INFO]-> Местное время замера: {current.Datetime}");
            }
            else
            {
                view.WriteLine("[ERROR] Failed to retrieve data. Please try again.");
            }

            view.WriteLine(new string('-', 40));

        }
    }
}
