using System;
using System.Collections.Generic;

namespace Resort_Hub.ViewModels.Admin
{
    public class DashboardViewModel
    {
        public int TotalBookings { get; set; }
        public int BookingsChange { get; set; } 

        public int TotalUsers { get; set; }
        public int UsersChange { get; set; }

        public decimal TotalRevenue { get; set; }
        public decimal RevenueChange { get; set; } 

        public decimal AverageBookingValue { get; set; }
        public int TotalVillas { get; set; }
        public double AverageRating { get; set; }
       

        public ChartDataViewModel ChartData { get; set; }

        public List<BookingViewModel> RecentBookings { get; set; }
        public List<RecentUserViewModel> RecentUsers { get; set; }

        public int TotalProperties { get; set; }
        public int ActiveBookings { get; set; }
        public double OccupancyRate { get; set; }
        public int AvailableVillas { get; set; }   
        public int CompletedBookings { get; set; }
        public int CancelledBookings { get; set; } 
        public double BookingCompletionRate { get; set; } 
        public DashboardViewModel()
        {
            ChartData = new ChartDataViewModel();
            RecentBookings = new List<BookingViewModel>();
            RecentUsers = new List<RecentUserViewModel>();
        }
    }

   
}