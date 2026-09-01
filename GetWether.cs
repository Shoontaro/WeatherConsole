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
        IMemoryCache cache;

        public GetWether()
        {
            apiKey = new GetApiKey().GetKey();
            cache = new MemoryCache(new MemoryCacheOptions());
        }

        public async Task GetWeatherAsync()
        {
            string location = "Minsk";
            string cacheKey = $"weather_{location.ToLower()}";

            var weather = await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

                Console.WriteLine("[СЕТЬ] Данных в кэше нет. Делаем реальный запрос через Flurl...");

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
                    Console.WriteLine($"[ОШИБКА API] Ошибка сети или некорректный запрос. Статус-код: {status}");

                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.Zero;
                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ОШИБКА] Непредвиденная ошибка: {ex.Message}");
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.Zero;
                    return null;
                }
            });

            if (weather?.CurrentConditions != null)
            {
                var current = weather.CurrentConditions;
                Console.WriteLine($"\n[УСПЕХ] Город (по базе): {weather.Address}");
                Console.WriteLine($"-> Температура: {current.Temp} °C (Ощущается как: {current.FeelsLike} °C)");
                Console.WriteLine($"-> Погода: {current.Conditions}");
                Console.WriteLine($"-> Влажность: {current.Humidity}%");
                Console.WriteLine($"-> Местное время замера: {current.Datetime}");
            }
            else
            {
                Console.WriteLine("[РЕЗУЛЬТАТ] Не удалось получить данные. Попробуйте еще раз.");
            }

            Console.WriteLine(new string('-', 40));

        }
    }
}
