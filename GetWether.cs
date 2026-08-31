using System;
using System.Collections.Generic;
using System.Text;
//using Flurl;
//using Flurl.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;


namespace WeatherConsole
{
    public class GetWether
    {
        public void Get()
        {

            string apiKey = new GetApiKey().GetKey();


        }
    }
}
