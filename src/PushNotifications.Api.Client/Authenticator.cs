using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Thinktecture.IdentityModel.Client;

namespace PushNotifications.Api.Client
{
    public sealed class Authenticator
    {
        readonly Uri authorizationEndpoint;
        readonly Options options;

        TokenResponse current;

        public Authenticator(Options options)
        {
            this.authorizationEndpoint = new Uri(options.Authority, "connect/token");
            this.options = options;
            current = new TokenResponse(HttpStatusCode.Unauthorized, "Unauthorized");
        }

        Authenticator(Authenticator authenticator, TokenResponse tokenResponse)
        {
            this.authorizationEndpoint = new Uri(authenticator.options.Authority, "connect/token");
            this.options = authenticator.options;
            current = tokenResponse;
            InitializedAt = DateTime.UtcNow;
        }

        public DateTime InitializedAt { get; private set; }

        public string AccessToken { get { return current.AccessToken; } }

        public string Error { get { return current.Error; } }

        public double ExpiresIn { get { return current.ExpiresIn - (DateTime.UtcNow - InitializedAt).TotalSeconds; } }

        public string HttpErrorReason { get { return current.HttpErrorReason; } }

        public HttpStatusCode HttpErrorStatusCode { get { return current.HttpErrorStatusCode; } }

        public string IdentityToken { get { return current.IdentityToken; } }

        public bool IsError { get { return current.IsError; } }

        public bool IsHttpError { get { return current.IsHttpError; } }

        public JObject Json { get { return current.Json; } }

        public string Raw { get { return current.Raw; } }

        public string RefreshToken { get { return current.RefreshToken; } }

        public string TokenType { get { return current.TokenType; } }

        public async Task<Authenticator> GetClientCredentialsAuthenticatorAsync()
        {
            var client = new OAuth2Client(authorizationEndpoint, options.ClientId, options.ClientSecret, OAuth2Client.ClientAuthenticationStyle.BasicAuthentication);
            TokenResponse tokenResponse = await client.RequestClientCredentialsAsync(options.Scope).ConfigureAwait(false);
            return new Authenticator(this, tokenResponse);
        }

        public sealed class Options
        {
            public Uri Authority { get; set; }

            public string ClientId { get; set; }

            public string ClientSecret { get; set; }

            public string Scope { get; set; }
        }
    }
}
