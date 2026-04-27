using Resort_Hub.DTOs.FontAwesomeIcon;

namespace Resort_Hub.Services.FontAwesome
{
    public interface IFontAwesomeService
    {
        //Task<List<string>> SearchIconsAsync(string query, int take = 20);
        Task<List<FontAwesomeIconDto>> SearchIconsAsync(string query);
    }
}
