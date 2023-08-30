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
        private readonly string connectionString;

        public Helper()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ToString();
        }

        public bool IsValidUser(string email, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT HashedPassword FROM Users WHERE Email = @Email";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    string hashedPassword = (string)command.ExecuteScalar();

                    if (hashedPassword != null)
                    {
                        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                    }
                }
            }

            return false;
        }

        public string GetUserRole(string email)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT Role FROM Users WHERE Email = @Email";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    object role = command.ExecuteScalar();
                    if (role != null)
                    {
                        return role.ToString();
                    }
                }
            }

            return null; // User not found or error occurred
        }

        public void InsertUser(RegisterModel user)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string insertQuery = "INSERT INTO Users (Name, Age, Place, Email, HashedPassword, Role) " +
                                     "VALUES (@Name, @Age, @Place, @Email, @HashedPassword, @Role)";

                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@Age", user.Age);
                    command.Parameters.AddWithValue("@Place", user.Place);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@HashedPassword", hashedPassword);
                    command.Parameters.AddWithValue("@Role", "User");

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}

