using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherConsole
{
    public class View : IView
    {
        public void Write(string text) => Console.Write(text);

        public void WriteLine(string text) => Console.WriteLine(text);

        public string Read() => Console.ReadLine() ?? "";
    }

    public class SpectreView : IView
    {
        public string Read() => Console.ReadLine() ?? "";

        public void Write(string text) => AnsiConsole.Markup(text);

        public void WriteLine(string text)
        {
            if (text.Contains("INFO")) {
                text += "[/]";
            }
            text.Replace("INFO", "green");

            AnsiConsole.MarkupLine(text);
        }

        
    }
}
