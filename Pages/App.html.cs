using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using netstrava.Models;
using Newtonsoft.Json;

namespace netstrava.Pages;

public class AppModel : PageModel
{
    private readonly ILogger<PageModel> _logger;
    public string Token {get; set; }
    public Athlete App {get;set;} = new Athlete();

    public AppModel(ILogger<AppModel> logger)
    {
        _logger = logger;
    }
    public async Task<IActionResult> OnGet()
    {
        Token = Request.Cookies["token"];
        if(String.IsNullOrWhiteSpace(Token))
            return Redirect("/Index");

        using(var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",Token);

            var response = await client.GetAsync("https://www.strava.com/api/v3/athlete");
            App = await response.Content.ReadFromJsonAsync<Athlete>();
            _logger.LogInformation("Recieved data for {0}", App?.FirstName);

            response = await client.GetAsync("https://www.strava.com/api/v3/athlete/activities");
            string strActs = await response.Content.ReadAsStringAsync();
            //var obj = JsonConvert.DeserializeObject(strActs, typeof(IEnumerable<StravaActivity>));
            var acts = await response.Content.ReadFromJsonAsync<IEnumerable<StravaActivity>>();
            if(acts is not null)
                App.Activities = acts;
        }

        return Page();
    }
}

public class StravaActivity
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("distance")]
    public double? Distance { get; set; }

    [JsonPropertyName("average_heartrate")]
    public decimal? AverageHeartRate { get; set; }

    [JsonPropertyName("average_watts")]
    public decimal? AverageWatts { get; set; }

    [JsonPropertyName("has_heartrate")]
    public bool HasHeartRate { get; set; } = false;
}
