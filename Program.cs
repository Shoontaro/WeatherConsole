using Microsoft.Extensions.Caching.Memory;
using WeatherConsole;

internal class Program
{
    private static void Main(string[] args)
    {
        using IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        IView view = new View();

        view.WriteLine("[green] Weather consile start...[/]");

        while (true)
        {
            new GetWether(view).GetWeatherAsync().Wait();
        }
    }
}