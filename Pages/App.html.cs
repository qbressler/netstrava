using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using netstrava.Models;

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
            _logger.LogInformation(App?.FirstName);
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("All is well so far");
            }
        }

        return Page();
    }
}
