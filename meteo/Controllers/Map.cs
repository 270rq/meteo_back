using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Drawing.Printing;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace meteo.Controllers
{
    [ApiController] 
    public class Map : ControllerBase
    {
        public static List<(string, float, float)> GetAllValuesFromMapTable(string month, string Name_flower, float X, float Y)
        {
            List<string> monthArr = new List<string> { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" };
            string nextMonth = monthArr[(monthArr.FindIndex(e => e == month)+ 1) % 12 + 1];
            string connectionString = "server=localhost;user id=root;password=1111;database=meteo";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            double latitude = X;
            double longitude = Y;
            string query = "SELECT * FROM map WHERE ST_Distance_Sphere(point(x, y), point(" + latitude + ", " + longitude + ")) <= 20000 and name_flower = '" + Name_flower +"' and (month = '"+month+"' or month = '"+nextMonth+"')";
            Console.WriteLine(query);
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = null;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
                return null;
            }
            List<(string, float, float)> values = new List<(string, float, float)>();

            while (reader.Read())
            {
                var value = ( reader.GetString(1), reader.GetFloat(2), reader.GetFloat(3));

                values.Add(value);
            }
            reader.Close();
            connection.Close();
            return values;

        }

        [HttpGet("/Map")]
        public List<meteo.Flower_map> Get(string month,string Name_flower, float X, float Y)
        {
            List<meteo.Flower_map> map = new List<meteo.Flower_map>();
            List<(string Name_flower, float X, float Y)> valuesList1 = GetAllValuesFromMapTable(month,Name_flower, X, Y);
            if (valuesList1 == null)
            {
                return null;   
            }
            foreach (var values in valuesList1)
            {
                meteo.Flower_map flower_Map = new meteo.Flower_map()
                {
                    NameFlower = values.Item1,
                    X = values.Item2,
                    Y = values.Item3
                };
                map.Add(flower_Map);
                   
            }
            return map;
        }
        [HttpGet("/Family")]
        public List<string> GetFam()
        {
            string connectionString = "server=localhost;user id=root;password=1111;database=meteo";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            string query = "SELECT * FROM family";
            Console.WriteLine(query);
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = null;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
                return null;
            }
            List<string> values = new List<string>();

            while (reader.Read())
            {
                var value = reader.GetString(0);

                values.Add(value);
            }
            reader.Close();
            connection.Close();
            return values;
        }
        [HttpGet("/plants")]
        public List <string> GetPlants(string Family) {
            string connectionString = "server=localhost;user id=root;password=1111;database=meteo";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            string query = "SELECT * FROM flower where family ='"+Family+"';";
            Console.WriteLine(query);
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = null;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
                return null;
            }
            List<string> values = new List<string>();

            while (reader.Read())
            {
                var value = reader.GetString(0);

                values.Add(value);
            }
            reader.Close();
            connection.Close();
            return values;
        }
    }
}
