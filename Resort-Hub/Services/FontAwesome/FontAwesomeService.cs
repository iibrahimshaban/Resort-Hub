using Microsoft.Extensions.Options;
using Resort_Hub.Configuration;
using Resort_Hub.DTOs.FontAwesomeIcon;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Resort_Hub.Services.FontAwesome
{
    public class FontAwesomeService : IFontAwesomeService
    {
        private readonly HttpClient _httpClient;
        private readonly FontAwesomeOptions _options;

        private string? _accessToken;
        private DateTime _expiresAt;

        private List<FontAwesomeIconDto>? _freeIconsCache;

        public FontAwesomeService(HttpClient httpClient, IOptions<FontAwesomeOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<List<FontAwesomeIconDto>> SearchIconsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new();

            var icons = await GetFreeIconsAsync();

            return icons.Where(x =>
                               x.Id.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               x.Label.Contains(query, StringComparison.OrdinalIgnoreCase))
                        .Take(20)
                        .ToList();
        }

        private async Task<List<FontAwesomeIconDto>> GetFreeIconsAsync()
        {
            if (_freeIconsCache != null)
                return _freeIconsCache;

            await LoadFreeIconsAsync();
            return _freeIconsCache!;
        }


        private async Task LoadFreeIconsAsync()
        {
            var accessToken = await GetAccessTokenAsync();

            var graphqlQuery = new
            {
                query = @"
                query {
                  release(version: ""7.x"") {
                    icons {
                      id
                      label
                      familyStylesByLicense {
                        free {
                          style
                        }
                      }
                    }
                  }
                }"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.fontawesome.com");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = new StringContent(
                JsonSerializer.Serialize(graphqlQuery),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"FontAwesome API Error: {json}");
            }

            using var doc = JsonDocument.Parse(json);

            var icons = doc.RootElement
                .GetProperty("data")
                .GetProperty("release")
                .GetProperty("icons")
                .EnumerateArray()
                .Where(icon =>
                {
                    var license = icon.GetProperty("familyStylesByLicense");

                    return license.TryGetProperty("free", out var free)
                           && free.ValueKind == JsonValueKind.Array
                           && free.GetArrayLength() > 0;
                })
                .Select(icon =>
                {
                    var free = icon.GetProperty("familyStylesByLicense")
                                   .GetProperty("free")[0];

                    var style = free.GetProperty("style").GetString();

                    return new FontAwesomeIconDto
                    {
                        Id = icon.GetProperty("id").GetString()!,
                        Label = icon.GetProperty("label").GetString()!,
                        Prefix = style switch
                        {
                            "solid" => "fa-solid",
                            "regular" => "fa-regular",
                            "brands" => "fa-brands",
                            _ => "fa-solid"
                        }
                    };
                })
                .ToList();

            _freeIconsCache = icons;
        }

        //public async Task<List<string>> SearchIconsAsync(string query, int take = 20)
        //{
        //    if (string.IsNullOrWhiteSpace(query))
        //        return new List<string>();

        //    var accessToken = await GetAccessTokenAsync();

        //    var requestBody = new
        //    {
        //        query = @"
        //        query($term:String!, $take:Int!) {
        //          search(version: ""7.x"", query:$term, first:$take) {
        //            id
        //          }
        //        }",
        //        variables = new
        //        {
        //            term = query,
        //            take
        //        }
        //    };

        //    using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.fontawesome.com");
        //    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        //    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //    request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");


        //    var response = await _httpClient.SendAsync(request);

        //    if (!response.IsSuccessStatusCode)
        //        return new List<string>();

        //    var json = await response.Content.ReadAsStringAsync();

        //    using var doc = JsonDocument.Parse(json);

        //    //SAFE CHECK (important)
        //    if (!doc.RootElement.TryGetProperty("data", out var data) ||
        //        data.ValueKind == JsonValueKind.Null ||
        //        !data.TryGetProperty("search", out var search) ||
        //        search.ValueKind != JsonValueKind.Array)
        //        {
        //            return new List<string>();
        //        }

        //    return search.EnumerateArray()
        //                 .Select(x => x.GetProperty("id").GetString()!)
        //                 .ToList();
        //}





        private async Task<string> GetAccessTokenAsync()
        {
            if (_accessToken != null && DateTime.UtcNow < _expiresAt)
                return _accessToken;


            var request = new HttpRequestMessage(HttpMethod.Post,
                "https://api.fontawesome.com/token");

            request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", _options.ApiToken);

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = new StringContent("{ }", Encoding.UTF8,"application/json");

            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);

            _accessToken = doc.RootElement
                              .GetProperty("access_token")
                              .GetString();

            _expiresAt = DateTime.UtcNow.AddSeconds(doc.RootElement.GetProperty("expires_in").GetInt32() - 30);

            return _accessToken!;
        }
    }
}
