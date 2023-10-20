using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace meteo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WeatherWeek : ControllerBase
    {
        public static List<int> GetAllValuesFromMenuTable(int City, DateTime Day)
        {
            string connectionString = "server=localhost;user id=root;password=1111;database=meteo";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            DateTime startDate = Day.AddDays(-7);
            string query = "SELECT * FROM menu WHERE city_id = " + City.ToString() + " AND id >= '" + startDate.ToString("yyyy-MM-dd") + "' AND id <= '" + Day.ToString("yyyy-MM-dd") + "'";

            MySqlCommand command = new MySqlCommand(query, connection);

            MySqlDataReader reader = command.ExecuteReader();

            List<int> values = new List<int>();

            while (reader.Read())
            {
                var value = reader.GetInt16(2);

                values.Add(value);
            }
            reader.Close();
            connection.Close();

            return values;

        }
        [HttpGet("/WeatherWeek")]
        public List<meteo.WeatherWeek> Get(DateTime Day, int City)
        {
            List<meteo.WeatherWeek> weatherDays = new List<meteo.WeatherWeek>();
            List<int> valuesList1 = GetAllValuesFromMenuTable(City, Day);
            foreach (var values in valuesList1)
            {
                meteo.WeatherWeek weatherWeek = new meteo.WeatherWeek
                {
                    TemperatureC = values,
                };
                weatherDays.Add(weatherWeek);
            }
            return weatherDays;
        }
    }
   
}

