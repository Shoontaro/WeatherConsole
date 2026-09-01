using Microsoft.Extensions.Caching.Memory;
using WeatherConsole;

internal class Program
{
    private static void Main(string[] args)
    {
        using IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        IView view = new SpectreView();

        view.WriteLine("[green] Приложение погоды на Flurl запущено.[/]");

        new GetWether(view).GetWeatherAsync().Wait();
    }
}