using System;
using System.Collections.Generic;
using System.Linq;
using HI.DevOps.DomainCore.Models;

namespace HI.DevOps.Application.BussinessManager
{
    public class WeatherForecastBm
    {
        #region Private

        private readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        #endregion

        public IEnumerable<WeatherForecast> GetWeatherForecast()
        {
            var rng = new Random();

            var vm = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });


            return vm;
        }
    }
}