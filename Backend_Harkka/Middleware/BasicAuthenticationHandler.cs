using Backend_Harkka.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Backend_Harkka.Middleware
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserAuthenticationService _userAuthenticationService;
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IUserAuthenticationService service) : base(options, logger, encoder, clock)
        {
            _userAuthenticationService = service;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string userName = "";
            string password = "";
            User? user;
            var endpoint = Context.GetEndpoint();

            var isAllowedAnonymous = endpoint?.Metadata.Any(x => x.GetType() == typeof(AllowAnonymousAttribute));
            var authorizeAttribute = endpoint?.Metadata.OfType<AuthorizeAttribute>().FirstOrDefault();
            var allowAnonymousAttribute = endpoint?.Metadata.OfType<AllowAnonymousAttribute>().FirstOrDefault();
            
            if (allowAnonymousAttribute != null || authorizeAttribute == null)
            {
                return AuthenticateResult.NoResult();
            }

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Authorization header missing");
            }
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialData = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialData).Split(new[] { ':' }, 2);
                userName = credentials[0];
                password = credentials[1];

                user = await _userAuthenticationService.Authenticate(userName, password);
                if (user ==  null)
                {
                    return AuthenticateResult.Fail("Unauthorized");
                }
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail("Unauthorized");
            }
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userName),
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}
