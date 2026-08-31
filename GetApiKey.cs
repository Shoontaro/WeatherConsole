using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace WeatherConsole
{
    public class GetApiKey
    {
        string ApiKey { get; set; }

        public GetApiKey()
        {
            var config = new ConfigurationBuilder()
               .AddUserSecrets<Program>() // Подключаем локальные секреты
               .AddEnvironmentVariables() // Подключаем переменные окружения для продакшена
               .Build();
            ApiKey = config["MyApiKey"] ?? string.Empty;
        }

        public string GetKey() => this.ApiKey;
    }
}
