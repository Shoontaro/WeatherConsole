using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherConsole
{
    public class VisualCrossingResponse
    {
        public string Address { get; set; } = string.Empty;
        public CurrentConditions? CurrentConditions { get; set; }
    }

    public class CurrentConditions
    {
        public string Datetime { get; set; } = string.Empty;
        public double Temp { get; set; }
        public double FeelsLike { get; set; }
        public double Humidity { get; set; }
        public string Conditions { get; set; } = string.Empty;
    }
}
