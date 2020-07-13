using System;
using System.Collections.Generic;
using System.Text;

namespace IOLApiClient.Auth.Repository.Abstractions.Configuration
{
    public interface ILoginRepositorySettings
    {
        string UserNameIOLClient { get; set; }
        string PasswordIOLClient { get; set; }
        string BaseUrl { get; set; }
    }
}
