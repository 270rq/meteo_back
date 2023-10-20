using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using BCryptNet = BCrypt.Net.BCrypt;
namespace meteo.Controllers
{
    [Route("user/[controller]")]
    [ApiController]
    public class loginReg : ControllerBase
    {
        [HttpPost("reg")]
        public int Post(RegUserModel user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string email = user.Email;
                    string password = BCryptNet.HashPassword(user.Password);
                    string connectionString = "server=localhost;user id=root;password=1111;database=meteo";
                    MySqlConnection connection = new MySqlConnection(connectionString);
                    connection.Open();
                    string query = "INSERT INTO users ( email, password) VALUES ( @Email, @Password)";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);
                    command.ExecuteNonQuery();
                    query = "SELECT id from users where email = @Email";
                    command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", email);
                    MySqlDataReader reader = command.ExecuteReader();
                    var value = 0;
                    if (reader.Read())
                    {
                        value = reader.GetInt32(0);
                    }
                    reader.Close();
                    connection.Close();
                    return value;
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Error: {0}", ex.ToString());
                    return -1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: {0}", ex.ToString());
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        [HttpPost("log")]
        public int PostLog(LogUserModel user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string email = user.Email;
                    string connectionString = "server=localhost;user id=root;password=1111;database=meteo";
                    MySqlConnection connection = new MySqlConnection(connectionString);
                    connection.Open();
                    string query = "SELECT * from users where email = @Email";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", email);
                    MySqlDataReader reader = command.ExecuteReader();
                    var value = 0;
                    var pass = "";
                    if (reader.Read())
                    {
                        value = reader.GetInt32(0);
                        pass = reader.GetString(2);
                    }
                    reader.Close();
                    connection.Close();
                    if (BCryptNet.Verify(user.Password, pass))
                    {
                        return value;
                    }
                    else
                    {
                        return -1;
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Error: {0}", ex.ToString());
                    return -1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: {0}", ex.ToString());
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
    }
}
