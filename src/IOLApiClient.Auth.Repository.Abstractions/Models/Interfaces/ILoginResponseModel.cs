using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace IOLApiClient.Auth.Repository.Abstractions.Models.Interfaces
{
    public interface ILoginResponseModel
    {
        string AccesToken { get; set; }
        string TokenType { get; set; }
        int ExpiresIn { get; set; }
        string RefreshToken { get; set; }
        string Issued { get; set; }
        string Expires { get; set; }
        string RefreshExpires { get; set; }
    }
}
