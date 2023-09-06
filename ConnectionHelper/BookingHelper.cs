using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
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

        public List<string> GetTurfLocation()
        {
            List<string> turfLocations = new List<string>();
            _connection.Open();
            
                using (SqlCommand command = new SqlCommand("GetLocation", _connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            string location = (string)reader["Location"];
                            turfLocations.Add(location);
                        }
                    }
                }
            _connection.Close();
            return turfLocations;
        }
        #region ListTurf
        public List<TurfModel> GetTurf(string location)
        {
            _connection.Open();

            string query = "SELECT Id, Name FROM Turfs WHERE Location = @Location";

            List<TurfModel> turfs = new List<TurfModel>();

            using (SqlCommand command = new SqlCommand("GetTurfByLocation", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Location", location);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TurfModel turf = new TurfModel
                        {
                            TurfId = (int)reader["Id"],
                            TurfName = (string)reader["Name"],
                        };
                        turfs.Add(turf);
                    }
                }
            }
            _connection.Close();
            return (turfs);
        }
        #endregion

        #region ListSport
        public List<SportModel> GetSport(int turfId)
        {
            _connection.Open();

            string query = "SELECT Id, Name FROM Sports WHERE TurfId = @TurfID";

            List<SportModel> sports = new List<SportModel>();

            using (SqlCommand command = new SqlCommand("GetSports", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TurfID", turfId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SportModel sport = new SportModel
                        {
                            SportId = (int)reader["Id"],
                            SportName = (string)reader["Name"]
                        };
                        sports.Add(sport);
                    }
                }
            }
            _connection.Close();
            return sports;
        }
        #endregion

        #region ListTimeSlots
        public List<TimeSlotModel> GetAvailableTimeSlots(int turfId, int sportId, DateTime date)
        {
            _connection.Open();

            string query = "SELECT Id, StartTime, EndTime FROM TimeSlots WHERE TurfId = @TurfID AND SportId = @SportID AND IsBooked = 0 AND SlotDate = @Date";

            List<TimeSlotModel> timeSlots = new List<TimeSlotModel>();

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@TurfID", turfId);
                command.Parameters.AddWithValue("@SportID", sportId);
                command.Parameters.AddWithValue("@Date", date.Date);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TimeSlotModel timeSlot = new TimeSlotModel
                        {
                            TimeSlotId = (int)reader["Id"],
                            StartTime = (DateTime)reader["StartTime"],
                            EndTime = (DateTime)reader["EndTime"],
                            //IsBooked = (bool)reader["IsBooked"]
                        };
                        timeSlots.Add(timeSlot);
                    }
                }
            }
            _connection.Close();
            return timeSlots;
        }
        #endregion

        public bool BookTimeSlots(List<int> timeSlotIds, int userId, int turfId, DateTime bookingDate)
        {
            _connection.Open();

            try
            {

                using (SqlTransaction transaction = _connection.BeginTransaction())
                {
                    foreach (int timeSlotId in timeSlotIds)
                    {
                        // Insert a booking record for each selected time slot
                        string query = "INSERT INTO Bookings (TurfId, TimeSlot, UserId, BookedDate) VALUES (@TurfId, @TimeSlotId, @UserId, @BookingDate)";
                        using (SqlCommand command = new SqlCommand(query, _connection, transaction))
                        {
                            command.Parameters.AddWithValue("@TurfId", turfId);
                            command.Parameters.AddWithValue("@TimeSlotId", timeSlotId);
                            command.Parameters.AddWithValue("@UserId", userId);
                            command.Parameters.AddWithValue("@BookingDate", bookingDate);

                            command.ExecuteNonQuery();
                        }

                        string updateQuery = "UPDATE TimeSlots SET IsBooked = 1 WHERE Id = @TimeSlotId";
                        using (SqlCommand updateCommand = new SqlCommand(updateQuery, _connection, transaction))
                        {
                            updateCommand.Parameters.AddWithValue("@TimeSlotId", timeSlotId);
                            updateCommand.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    _connection.Close();
                    return true; // Booking successful
                }
               
            }
            catch (Exception)
            {
                _connection.Close();
                return false; // Booking failed
            }

        }

        public int GetUserIdByEmail(string userEmail)
        {
            _connection.Open();

            int userId = -1;

            string query = "SELECT Id FROM Users WHERE Email = @Email";

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@Email", userEmail);
                object result = command.ExecuteScalar();

                Console.WriteLine($"Query Result: {result}");

                if (result != null && int.TryParse(result.ToString(), out userId))
                {
                    Console.WriteLine($"Parsed UserId: {userId}");

                    return userId;
                }
            }
            _connection.Close();

            return userId; // User not found or error occurred
        }

    }
}