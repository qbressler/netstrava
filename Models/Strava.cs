
using System.Text.Json.Serialization;

namespace netstrava.Models;

public class Strava
{
    [JsonPropertyName("access_token")]
    public string? Token {get;set;}

    [JsonPropertyName("athlete")]
    public Athlete? Athlete {get;set;} = new Athlete();
}

public class Athlete
{
    [JsonPropertyName("profile")]
    public string? ProfilePicture {get;set;}

    [JsonPropertyName("firstname")]
    public string? FirstName {get;set;}

    [JsonPropertyName("lastname")]
    public string? LastName {get;set;}

    [JsonPropertyName("state")]
    public string? State {get;set;}

    [JsonPropertyName("friend_count")]
    public int Followers {get;set;}

}