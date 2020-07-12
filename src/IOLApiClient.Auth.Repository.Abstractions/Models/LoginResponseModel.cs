using IOLApiClient.Auth.Repository.Abstractions.Models.Interfaces;
using System;
using System.Text.Json.Serialization;

namespace IOLApiClient.Auth.Repository.Abstractions.Models
{
    public class LoginResponseModel : ILoginResponseModel
    {
        [JsonPropertyName("access_token")]
        public string AccesToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName(".issued")]
        public string Issued { get; set; }

        [JsonPropertyName(".expires")]
        public string Expires { get; set; }

        [JsonPropertyName(".refreshexpires")]
        public string RefreshExpires { get; set; }

        public DateTime IssuedDateTime { get; set; }
        public DateTime ExpiresDateTime { get; set; }
        public DateTime RefreshExpiresDateTime { get; set; }
    }
}
