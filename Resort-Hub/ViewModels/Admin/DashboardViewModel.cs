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

        public ChartDataViewModel ChartData { get; set; }

        public List<RecentBookingViewModel> RecentBookings { get; set; }
        public List<RecentUserViewModel> RecentUsers { get; set; }

        public int TotalProperties { get; set; }
        public int ActiveBookings { get; set; }
        public decimal OccupancyRate { get; set; }

        public DashboardViewModel()
        {
            ChartData = new ChartDataViewModel();
            RecentBookings = new List<RecentBookingViewModel>();
            RecentUsers = new List<RecentUserViewModel>();
        }
    }

   
}