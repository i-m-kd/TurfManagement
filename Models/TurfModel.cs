using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TurfManagement.Models
{
    public class TurfModel
    {
        public List<TurfModel> Turfs { get; set; }
        public int SelectedTurfId { get; set; }
        public int TurfId { get; set; }
        public string TurfName { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
    }

    public class TurfListViewModel
    {
        public List<TurfModel> Turfs { get; set; }
        public int SelectedTurfId { get; set; }
    }

    public class SportModel
    {
        public int SportId { get; set; }
        public string SportName { get; set; }
        public int TurfId { get; set; }
    }

    public class TimeSlotModel
    {
        public int TimeSlotId { get; set; }
        public int TurfId { get; set; }
        public int SportId { get; set; }
        public DateTime SlotDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsBooked { get; set; }
    }

    public class BookingModel
    {
        public int BookingId { get; set; }
        public int TurfId { get; set; }
        public int TimeSlotId { get; set; }
        public int UserId { get; set; }
        public DateTime BookingDate { get; set; }
    }
}