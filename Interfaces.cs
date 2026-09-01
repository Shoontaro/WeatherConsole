using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherConsole
{
    public interface IView
    {
        void Write(string text);
        void WriteLine(string text);
        string Read();
    }
}
