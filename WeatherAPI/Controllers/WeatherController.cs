using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeatherAPI.Models;

namespace WeatherAPI.Controllers
{
    public class WeatherController : Controller
    {
        public WeatherModel FillCity()
        {
            WeatherModel weathers = new WeatherModel();

            weathers.cities = new Dictionary<string, string>();
            weathers.cities.Add("Kharkiv", "706483");
            weathers.cities.Add("Kyiv", "703448");
            weathers.cities.Add("Poltava", "696643");
            return weathers;
        }
        // GET: Weather
        public ActionResult Index()
        {
            WeatherModel weather = FillCity();
            return View(weather);
        }
        [HttpPost]
        public ActionResult Index(WeatherModel weather, string cities)
        {
            weather = FillCity();

            if (cities != null)
            {
                /*Calling API http://openweathermap.org/api */
                string apiKey = "fb59ff231e41ec1d3ab5c10bb667052d";
                HttpWebRequest apiRequest =
                WebRequest.Create("http://api.openweathermap.org/data/2.5/weather?id=" +
                cities + "&appid=" + apiKey + "&units=metric") as HttpWebRequest;

                string apiResponse = "";
                using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    apiResponse = reader.ReadToEnd();
                }

                ResponseWeather rootObject = JsonConvert.DeserializeObject<ResponseWeather>(apiResponse);

                StringBuilder sb = new StringBuilder();
                sb.Append("<table><tr><th>Weather Description</th></tr>");
                sb.Append("<tr><td>City:</td><td>" +
                rootObject.name + "</td></tr>");
                sb.Append("<tr><td>Country:</td><td>" +
                rootObject.sys.country + "</td></tr>");
                sb.Append("<tr><td>Wind:</td><td>" +
                rootObject.wind.speed + " Km/h</td></tr>");
                sb.Append("<tr><td>Current Temperature:</td><td>" +
                rootObject.main.temp + " °C</td></tr>");
                sb.Append("<tr><td>Humidity:</td><td>" +
                rootObject.main.humidity + "</td></tr>");
                sb.Append("<tr><td>Weather:</td><td>" +
                rootObject.weather[0].description + "</td></tr>");
                sb.Append("</table>");
                weather.apiResponse = sb.ToString();
            }
            else
            {
                if (Request.Form["submit"] != null)
                {
                    weather.apiResponse = "► Select City";
                }
            }
            return View(weather);
        }
    }
}