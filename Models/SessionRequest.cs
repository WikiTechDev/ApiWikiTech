using System.Text.Json.Serialization;

namespace ApiWikiTech.Models
{
    public class SessionRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

    }

    public class RegisterRequest
    {
        [JsonPropertyName("identification")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        [JsonPropertyName("contact_phone")]
        public string Phone { get; set; } = string.Empty;
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }
}
