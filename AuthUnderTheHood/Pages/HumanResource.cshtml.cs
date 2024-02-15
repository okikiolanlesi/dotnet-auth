using System.Net.Http.Headers;
using System.Text.Json;
using AuthUnderTheHood.Authorization;
using AuthUnderTheHood.DTO;
using AuthUnderTheHood.Pages.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthUnderTheHood.Pages
{
    [Authorize(Policy = "HR")]
    public class HumanResourceModel : PageModel
    {

        [BindProperty]
        public IEnumerable<WeatherForecastDTO>? WeatherForecasts { get; set; }
        public HumanResourceModel(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory;
        }

        public IHttpClientFactory HttpClientFactory { get; }

        public async Task OnGetAsync()
        {

            var httpClient = HttpClientFactory.CreateClient("mywebapi");
            var jwtToken = new JwtDTO();
            //get the token from session
            var strTokenObj = HttpContext.Session.GetString("jwt");

            if (string.IsNullOrEmpty(strTokenObj))
            {

                jwtToken = await GetToken(httpClient);
            }
            else
            {
                jwtToken = JsonSerializer.Deserialize<JwtDTO>(strTokenObj) ?? new JwtDTO();
            }

            if (jwtToken.ExpiresAt < DateTime.Now || string.IsNullOrEmpty(jwtToken.AccessToken))
            {
                // authenticate with the webapi and get token
                jwtToken = await GetToken(httpClient);

            }


            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken?.AccessToken);
            WeatherForecasts = await httpClient.GetFromJsonAsync<IEnumerable<WeatherForecastDTO>>("weatherforecast");
        }

        private async Task<JwtDTO> GetToken(HttpClient httpClient)
        {
            System.Console.WriteLine("Getting token");
            // authenticate with the webapi and get token
            var res = await httpClient.PostAsJsonAsync("auth", new Credential { Username = "admin", Password = "password" });
            res.EnsureSuccessStatusCode();
            string strJwt = await res.Content.ReadAsStringAsync();
            HttpContext.Session.SetString("jwt", strJwt);
            return JsonSerializer.Deserialize<JwtDTO>(strJwt);
        }
    }
}


