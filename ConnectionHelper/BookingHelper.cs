using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TurfManagement.Models;

namespace TurfManagement.ConnectionHelper
{
    public class BookingHelper
    {
        private readonly SqlConnection _connection;

        public BookingHelper()
        {
            _connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnection"].ToString());
        }

        #region ListTurf
        public List<TurfModel> GetTurf()
        {
            _connection.Open();

            string query = "SELECT Id, Name, Location FROM Turfs";

            List<TurfModel> turfs = new List<TurfModel>();

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TurfModel turf = new TurfModel
                        {
                            TurfID = (int)reader["Id"],
                            TurfName = (string)reader["Name"],
                            Location = (string)reader["Location"]
                        };
                        turfs.Add(turf);
                    }
                }
            }
            return (turfs);
        }
        #endregion

        public List<SportModel> GetSport(int turfId)
        {
            _connection.Open();

            string query = "SELECT Id, Name FROM Sports WHERE TurfId = @TurfID";

            List<SportModel> sports = new List<SportModel>();

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@TurfID", turfId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SportModel sport = new SportModel
                        {
                            SportID = (int)reader["Id"],
                            SportName = (string)reader["Name"]
                        };
                        sports.Add(sport);
                    }
                }
            }

            return sports;
        }

    }
}