using Microsoft.Extensions.Caching.Memory;
using WeatherConsole;

internal class Program
{
    private static void Main(string[] args)
    {
        using IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        
        Console.WriteLine("Приложение погоды на Flurl запущено.");

        new GetWether().GetWeatherAsync().Wait();
    }
}