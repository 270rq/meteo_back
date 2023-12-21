using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Cms;
using System;

namespace meteo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherDay : ControllerBase
    {
        public static List<(TimeSpan, TimeSpan)> GetAllValuesFromSunTable(String City, String Region, DateTime Day)
        {
            string connectionString = "server=localhost;user id=root;password=1111;database=meteo";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            string city_idQ = "SELECT  * FROM cityWithRegion WHERE name= '" + City + "' and region_name = '" + Region + "' ;";
                MySqlCommand command = new MySqlCommand(city_idQ, connection);

            MySqlDataReader reader = command.ExecuteReader();
            int city_id = 3;
            if (reader.Read())
            {
                city_id = reader.GetInt32(0);
                // use the city_id value here
            }

            reader.Close();
            string query = "SELECT * FROM sun where city_id=" + city_id.ToString() + " and date='" + Day.ToString("yyyy-MM-dd") +"'";
             command = new MySqlCommand(query, connection);

            reader = command.ExecuteReader();

            List<( TimeSpan, TimeSpan)> values = new List<( TimeSpan, TimeSpan)>();

            while (reader.Read())
            {
                var value = (reader.GetTimeSpan(3), reader.GetTimeSpan(2));
                values.Add(value);
            }

            reader.Close();
            connection.Close();

            return values;
        }
         public static List<( int, float,float, float, string, int)> GetAllValuesFromMenuTable(string City, string Region, DateTime Day)
        {
            string connectionString = "server=localhost;user id=root;password=1111;database=meteo";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            string city_idQ = "SELECT  * FROM cityWithRegion WHERE name= '" + City + "' and region_name = '" + Region + "' ;";
                MySqlCommand command = new MySqlCommand(city_idQ, connection);

            MySqlDataReader reader = command.ExecuteReader();
            int city_id = 3;
            if (reader.Read())
            {
                city_id = reader.GetInt32(0);
                // use the city_id value here
            }

            reader.Close();
            string query = "SELECT * FROM menu where city_id=" + city_id.ToString()+" and Date(id)='" +Day.ToString("yyyy-MM-dd") +"'";

            command = new MySqlCommand(query, connection);

            reader = command.ExecuteReader();

            List<( int, float, float,float, string, int)> values = new List<(int, float, float, float, string, int)>();

            while (reader.Read()) 
            {
                var value = (reader.GetInt16(2), reader.GetFloat(3), reader.GetFloat(4), reader.GetFloat(5), reader.GetString(6), reader.GetInt16(7));
                
                values.Add(value);
            }
            reader.Close();
            connection.Close();

            return values;
        }




        private readonly ILogger<WeatherDay> _logger;

        public WeatherDay(ILogger<WeatherDay> logger)
        {
            _logger = logger;
        }
        public static int getCityId(string City, string Region)
        {
            string connectionString = "server=localhost;user id=root;password=1111;database=meteo";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string city_idQ = "SELECT id FROM cityWithRegion WHERE name = @City AND region_name = @Region";
                using (MySqlCommand command = new MySqlCommand(city_idQ, connection))
                {
                    command.Parameters.AddWithValue("@City", City);
                    command.Parameters.AddWithValue("@Region", Region);
                    int city_id = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                    return city_id;
                }
            }
        }
        [HttpPost("SunCycle", Name = "insertSunData")]
        public string PostSun([FromBody] SunData sunData)
        {
            DateTime? Day = sunData.Day;
            string City = sunData.City;
            string Region = sunData.Region;
            TimeSpan sunRise = sunData.SunRise;
            TimeSpan sunSet = sunData.SunSet;

            string connectionString = "server=localhost;user id=root;password=1111;database=meteo";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                int city_id = getCityId(City, Region);

                if (city_id != 0)
                {
                    string insertQ = "INSERT INTO sun (date, city_id, sunrise,sunset) VALUES (@Day, @city_id, @sunrise, @sunset)";
                    using (MySqlCommand insertCommand = new MySqlCommand(insertQ, connection))
                    {
                        DateTime dayOnly = Day?.Date ?? DateTime.MinValue;
                        insertCommand.Parameters.AddWithValue("@Day", dayOnly);
                        insertCommand.Parameters.AddWithValue("@city_id", city_id);
                        insertCommand.Parameters.AddWithValue("@sunrise", sunRise);
                        insertCommand.Parameters.AddWithValue("@sunset", sunSet);

                        int rowsAffected = insertCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return "Ok";
                        }
                        else
                        {
                            return "Not ok";
                        }
                    }
                }
                else
                {
                    return "Can't find city";
                }

            }

        }


        [HttpPost(Name = "Weather")]
        public string PostWeather([FromBody] WeatherData weatherData)
        {
            DateTime? Day = weatherData.Day;
            string City = weatherData.City;
            string Region = weatherData.Region;
            int Temperature = weatherData.Temperature;
            float Precipitation = weatherData.Precipitation;
            float Uv_index = weatherData.Uv_index;
            float Wind_speed = weatherData.Wind_speed;
            string Wind_direction = weatherData.Wind_direction;
            float Visibility = weatherData.Visibility;

            string connectionString = "server=localhost;user id=root;password=1111;database=meteo";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                int city_id = getCityId(City, Region);

                    if (city_id != 0)
                    {
                        string insertQ = "INSERT INTO menu (id, city_id, temperature, precipitation, uv_index, wind_speed, wind_direction, visibility) VALUES (@Day, @city_id, @Temperature, @Precipitation, @Uv_index, @Wind_speed, @Wind_direction, @Visibility)";
                        using (MySqlCommand insertCommand = new MySqlCommand(insertQ, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@Day", Day ?? (object)DBNull.Value);
                            insertCommand.Parameters.AddWithValue("@city_id", city_id);
                            insertCommand.Parameters.AddWithValue("@Temperature", Temperature);
                            insertCommand.Parameters.AddWithValue("@Precipitation", Precipitation);
                            insertCommand.Parameters.AddWithValue("@Uv_index", Uv_index);
                            insertCommand.Parameters.AddWithValue("@Wind_speed", Wind_speed);
                            insertCommand.Parameters.AddWithValue("@Wind_direction", Wind_direction);
                            insertCommand.Parameters.AddWithValue("@Visibility", Visibility);

                            int rowsAffected = insertCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                return "Ok";
                            }
                            else
                            {
                                return "Not ok";
                            }
                        }
                    }
                    else
                    {
                        return "Can't find city";
                    }
                
            }
        }
        [HttpGet(Name = "WeatherDay")]
        public List<meteo.WeatherDay> Get(DateTime Day, string City, string Region)
        {
            List<meteo.WeatherDay> weatherDays = new List<meteo.WeatherDay>();
            List<(TimeSpan, TimeSpan)> valuesList = GetAllValuesFromSunTable(City, Region, Day);
            List<(int, float, float, float, string, int)> valuesList1 = GetAllValuesFromMenuTable(City,Region, Day);
            try
            {
                foreach (var values in valuesList1)
                {
                    meteo.WeatherDay weatherDay = new meteo.WeatherDay
                    {
                       
                        TemperatureC = values.Item1,
                        Precipitation = values.Item2,
                        UvIndex = values.Item3,
                        SpeedWind = values.Item4,
                        TypeWind = values.Item5,
                        Visibility = values.Item6,
                        Sunrise = valuesList[0].Item1,
                        Sunset = valuesList[0].Item2,
                        DewPoint = CalculateDewPoint(values.Item1, values.Item2),
                        WeatherSensation = GetWeatherSensation(values.Item1, values.Item2, values.Item4),
                        Visi_state = CalculateVisibility(values.Item6)
                    };
                    weatherDays.Add(weatherDay);
                }
                return weatherDays;
            }
            catch
            {
                return null;
            }
        }

        public static int CalculateDewPoint(double temperature, double precipitation)
        {
            double dewPoint = temperature - ((100 - precipitation) / 5);
            int result = (int)Math.Round(dewPoint);
            return result;
        }
        public static string GetWeatherSensation(double temperature, double precipitation, double windSpeed)
        {
            double heatIndex = CalculateHeatIndex(temperature, precipitation);
            double windChill = CalculateWindChill(temperature, windSpeed);

            if (heatIndex >= 40)
            {
                return "Очень жарко: высокая температура и влажность";
            }
            else if (windChill <= -27)
            {
                return "Очень холодно: низкая температура и сильный ветер";
            }
            else if (temperature >= 30)
            {
                return "Жарко: высокая температура";
            }
            else if (temperature >= 20)
            {
                return "Тепло: комфортная температура";
            }
            else if (temperature >= 10)
            {
                return "Прохладно: низкая температура";
            }
            else 
            {
                return "Холодно: очень низкая температура";
            }
        }

        private static double CalculateHeatIndex(double temperature, double precipitation)
        {
            double c1 = -8.78469475556;
            double c2 = 1.61139411;
            double c3 = 2.33854883889;
            double c4 = -0.14611605;
            double c5 = -0.012308094;
            double c6 = -0.0164248277778;
            double c7 = 0.002211732;
            double c8 = 0.00072546;
            double c9 = -0.000003582;

            double T = temperature;
            double RH = precipitation;

            double heatIndex = c1 + c2 * T + c3 * RH + c4 * T * RH + c5 * T * T + c6 * RH * RH + c7 * T * T * RH + c8 * T * RH * RH + c9 * T * T * RH * RH;

            return heatIndex;
        }

        private static double CalculateWindChill(double temperature, double windSpeed)
        {
            double V = windSpeed;
            double T = temperature;

            double windChill = 13.12 + 0.6215 * T - 11.37 * Math.Pow(V, 0.16) + 0.3965 * T * Math.Pow(V, 0.16);

            return windChill;
        }
        public static string CalculateVisibility(int visibility)
        {
            if (visibility >= 0 && visibility < 50)
            {
                return "совершенно неясно";
            }
            else if (visibility >= 50 && visibility < 100)
            {
                return "неясно";
            }
            else if (visibility >= 100 && visibility < 200)
            {
                return "местами ясно";
            }
            else if (visibility >= 200 && visibility < 300)
            {
                return "ясно";
            }
            else
            {
                return "совершенно ясно";
            }
        }
    }
   
}