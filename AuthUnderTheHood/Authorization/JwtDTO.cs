using System.Text.Json.Serialization;

namespace AuthUnderTheHood.Authorization;

public class JwtDTO
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
    [JsonPropertyName("expires_at")]
    public DateTime ExpiresAt { get; set; }

}
