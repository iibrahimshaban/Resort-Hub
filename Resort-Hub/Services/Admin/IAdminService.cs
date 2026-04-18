using Resort_Hub.ViewModels.Admin;
using System.Threading.Tasks;

namespace ResortHub.Services
{
    public interface IAdminService
    {
        Task<DashboardViewModel> GetDashboardDataAsync();
        Task<ChartDataViewModel> GetChartDataAsync(int days = 30);
    }
}