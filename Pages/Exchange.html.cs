using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using netstrava.Models;

namespace netstrava.Pages;

public class ExchangeModel: PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly AppSettings _appSettings; 
    public Strava Strava {get;set;} = new Strava();
    public string? ErrorMsg { get; set; } = String.Empty;

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
            if (result.IsSuccessStatusCode)
            {
                Strava? st = await result!.Content!.ReadFromJsonAsync<Strava>();
                Strava = st;
                var cookieOptions = new CookieOptions()
                {
                    Expires = DateTime.Now.AddMinutes(10),
                };

                if (Strava is not null)
                    Response.Cookies.Append("token", Strava?.Token, cookieOptions);

                return Redirect("/App");
            }
            else
            {
                var responseObject = await result.Content.ReadFromJsonAsync<Response>();
                return Redirect($"/?ErrorMsg={responseObject?.Message}");
            }
        }
    
    }
}


internal class Response
{
    public string Message { get; set; }
}