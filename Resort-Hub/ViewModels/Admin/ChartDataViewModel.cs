namespace Resort_Hub.ViewModels.Admin
{
    public class ChartDataViewModel
    {
        public List<string> Dates { get; set; } 
        public List<int> BookingCounts { get; set; } 
        public List<int> MemberCounts { get; set; } 
        public ChartDataViewModel()
        {
            Dates = new List<string>();
            BookingCounts = new List<int>();
            MemberCounts = new List<int>();
        }
    }
}
