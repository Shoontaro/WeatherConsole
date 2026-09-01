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
        IView view;

        public GetWether(IView view)
        {
            apiKey = new GetApiKey().GetKey();
            cache = new MemoryCache(new MemoryCacheOptions());
            this.view = view;
        }

        public async Task GetWeatherAsync()
        {
            string location = "Minsk";
            string cacheKey = $"weather_{location.ToLower()}";

            var weather = await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

                view.WriteLine("[bold yellow] Данных в кэше нет. Делаем реальный запрос через Flurl...[/]");

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
                    view.WriteLine($"[red]--ОШИБКА API-- Ошибка сети или некорректный запрос. Статус-код: {status}[/]");

                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.Zero;
                    return null;
                }
                catch (Exception ex)
                {
                    view.WriteLine($"[red]--ОШИБКА-- Непредвиденная ошибка: {ex.Message}[/]");
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.Zero;
                    return null;
                }
            });

            if (weather?.CurrentConditions != null)
            {
                var current = weather.CurrentConditions;
                view.WriteLine($"\n[green]  Город (по базе): {weather.Address}[/]");
                view.WriteLine($"[green]-> Температура: {current.Temp} °C (Ощущается как: {current.FeelsLike} °C)[/]");
                view.WriteLine($"[green]-> Погода: {current.Conditions}[/]");
                view.WriteLine($"[green]-> Влажность: {current.Humidity}%[/]");
                view.WriteLine($"[green]-> Местное время замера: {current.Datetime}[/]");
            }
            else
            {
                view.WriteLine("[red] Не удалось получить данные. Попробуйте еще раз.[/]");
            }

            Console.WriteLine(new string('-', 40));

        }
    }
}
