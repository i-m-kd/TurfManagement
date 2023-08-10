using System.Configuration;
using System.Data.SqlClient;
using System.Web.Helpers;
using TurfManagement.Models;
using System.Security.Cryptography;
using System.Text;

namespace TurfManagement.ConnectionHelper
{
    public class Helper
    {
        private readonly SqlConnection _connection;
        public Helper()
        {
            _connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnection"].ToString());
        }

        public bool IsValidUser(string email, string password)
        {
            _connection.Open();

            string query = "SELECT HashedPassword FROM Users WHERE Email = @Email";
            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Email", email);

                string hashedPassword = (string)command.ExecuteScalar();

                if (hashedPassword != null)
                {
                    return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                }
            }

            _connection.Close();
            return false;
        }
        public void InsertUser(RegisterModel user)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

            {
                _connection.Open();

                string insertQuery = "INSERT INTO Users (Name, Age, Place, Email, HashedPassword) " +
                                     "VALUES (@Name, @Age, @Place, @Email, @HashedPassword)";

                using (SqlCommand command = new SqlCommand(insertQuery, _connection))
                {
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@Age", user.Age);
                    command.Parameters.AddWithValue("@Place", user.Place);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@HashedPassword", hashedPassword);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
