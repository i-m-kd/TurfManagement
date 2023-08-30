using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Management;
using TurfManagement.Models;

namespace TurfManagement.ConnectionHelper
{
    public class AdminHelper
    {
        private readonly SqlConnection _connection;

        public AdminHelper()
        {
            _connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnection"].ToString());
        }

        #region AddNewTurf
        public int AddTurf(TurfModel turf)
        {
            int result = 0;

            using (SqlCommand command = new SqlCommand("AddTurf", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Name", turf.TurfName);
                command.Parameters.AddWithValue("@Location", turf.Location);
                command.Parameters.AddWithValue("@Description", turf.Description);

                SqlParameter resultParameter = new SqlParameter("@Result", SqlDbType.Int);
                resultParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(resultParameter);

                try
                {
                    _connection.Open();
                    command.ExecuteNonQuery();

                    string query = "SELECT MAX(Id) FROM Turfs";
                    using (SqlCommand idCommand = new SqlCommand(query, _connection))
                    {
                        int turfId = Convert.ToInt32(idCommand.ExecuteScalar());
                        HttpContext.Current.Session["LastAddedTurfId"] = turfId;
                    }

                    result = (int)resultParameter.Value;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627)
                        result = -1;
                    else
                        throw new Exception("An error occurred while adding the turf.");
                }
                finally
                {
                    _connection.Close();
                }
            }
            return result;
        }
        #endregion

        #region AddNewSport
        public int AddSport(SportModel sport)
        {
            int result = 0;

            int turfId = (int)HttpContext.Current.Session["LastAddedTurfId"];
            sport.TurfId = turfId;

            using (SqlCommand command = new SqlCommand("AddSport", _connection))
            {
                int insertedId = 0;

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Name", sport.SportName);
                command.Parameters.AddWithValue("@TurfId", sport.TurfId);

                SqlParameter resultParameter = new SqlParameter("Result", SqlDbType.Int);
                resultParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(resultParameter);

                //Get the Id of last added sport
                SqlParameter sportIdParameter = new SqlParameter("InsertedSportId", SqlDbType.Int);
                sportIdParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(sportIdParameter);

                try
                {
                    _connection.Open();
                    command.ExecuteNonQuery();
                    result = (int)resultParameter.Value;
                    insertedId = (int)sportIdParameter.Value;
                    HttpContext.Current.Session["LastAddedSportId"] = insertedId;

                }
                catch (SqlException)
                {
                    throw new Exception("An error occurred while adding the sport.");
                }
                finally
                {
                    _connection.Close();
                }
            }

            return result;
        }
        #endregion

        public int AddTimeSlot(TimeSlotModel timeSlot)
        {

            int result = 0;
            int turfId = (int)HttpContext.Current.Session["LastAddedTurfId"];
            int sportId = (int)HttpContext.Current.Session["LastAddedSportId"];

            timeSlot.TurfId = turfId;
            timeSlot.SportId = sportId;

            using (SqlCommand command = new SqlCommand("AddTimeSlot", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@TurfId", timeSlot.TurfId);
                command.Parameters.AddWithValue("@SportId", timeSlot.SportId);
                command.Parameters.AddWithValue("@SlotDate", timeSlot.SlotDate.Date);
                command.Parameters.AddWithValue("@StartTime", timeSlot.StartTime);
                command.Parameters.AddWithValue("@EndTime", timeSlot.EndTime);
                command.Parameters.AddWithValue("@IsBooked", false);

                SqlParameter resultParameter = new SqlParameter("Result", SqlDbType.Int);
                resultParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(resultParameter);

                try
                {
                    _connection.Open();
                    command.ExecuteNonQuery();
                    result = (int)resultParameter.Value;
                }
                catch (SqlException ex)
                {
                    Log.Error(ex, "An error occurred");

                    throw new Exception("An error occurred while adding the timeslot.");
                }
                finally
                {
                    _connection.Close();
                }
            }
            return result;
        }

    }
}