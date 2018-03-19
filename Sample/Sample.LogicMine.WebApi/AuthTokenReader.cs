using System;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Sample.LogicMine.WebApi
{
  /// <summary>
  /// This is just a basic example of how you may extract identity information from a requests header.
  /// </summary>
  public class AuthTokenReader : IAuthTokenReader
  {
    private const string AuthorisationHeaderName = "Authorization";
    private const string BearerSchemeName = "Bearer";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthTokenReader(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public string GetToken()
    {
      var authHeader = _httpContextAccessor.HttpContext.Request.Headers[AuthorisationHeaderName].FirstOrDefault();
      if (authHeader != null)
      {
        var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);
        if (authHeaderVal.Scheme.Equals(BearerSchemeName, StringComparison.OrdinalIgnoreCase))
          return Uri.UnescapeDataString(authHeaderVal.Parameter);
      }

      return null;
    }
  }
}
