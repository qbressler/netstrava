using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using netstrava.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace netstrava.Pages;

public class ExchangeModel: PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly AppSettings _appSettings; 
    public Strava Strava {get;set;} = new Strava();
    public string? ProfilePicPath {get;set;} = "Default";

    public ExchangeModel(ILogger<IndexModel> logger, IOptions<AppSettings> options)
    {
        _logger = logger;
        _appSettings = options.Value;
    }

    public async Task<IActionResult> OnGet([FromQuery]string code)
    {

        var requestObject = new
        {
            client_id = 18876,
            client_secret = _appSettings.ClientSecret,
            grant_type = "authorization_code",
            code = code,
        };

        JsonContent content = JsonContent.Create(requestObject);
        using(var client = new HttpClient())
        {
            var result = await client.PostAsync("https://www.strava.com/oauth/token", content);
            if(result! is not null)
                Strava = await result!.Content!.ReadFromJsonAsync<Strava>();

            ProfilePicPath = Strava?.Athlete?.ProfilePicture;
            Console.WriteLine(ProfilePicPath);
        }
    
        var cookieOptions = new CookieOptions()
        {
            Expires = DateTime.Now.AddMinutes(10),
        };
        Response.Cookies.Append("token",Strava?.Token, cookieOptions);

        return Redirect("/App");
    }

}