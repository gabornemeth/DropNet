using RestSharp.Portable.Authenticators.OAuth2.Infrastructure;
using System;

namespace RestSharp.Portable.Authenticators.OAuth2.Client
{
    public class DropboxClient : OAuth2Client
    {
        public const string ApiBaseUrl = "https://api.dropbox.com";
        public const string ApiContentBaseUrl = "https://api-content.dropbox.com";
        public const string ApiNotifyUrl = "https://api-notify.dropbox.com";
        public const string Version = "1";

        /// <summary>
        /// To use Dropbox API in sandbox mode (app folder access) set to true
        /// </summary>
        public bool UseSandbox { get; set; }

        private const string SandboxRoot = "sandbox";
        private const string DropboxRoot = "dropbox";

        /// <summary>
        /// Gets the directory root for the requests (full or sandbox mode)
        /// </summary>
        public string Root
        {
            get { return UseSandbox ? SandboxRoot : DropboxRoot; }
        }

        public DropboxClient(IRequestFactory factory, RestSharp.Portable.Authenticators.OAuth2.Configuration.IClientConfiguration configuration)
            : base(factory, configuration)
        {
            //Default to full access
            UseSandbox = false;
        }

        protected override Endpoint AccessCodeServiceEndpoint
        {
            get
            {
                return new Endpoint
                {
                    BaseUri = "https://www.dropbox.com",
                    Resource = string.Format("/{0}/oauth2/authorize", Version)
                };
            }
        }

        protected override Endpoint AccessTokenServiceEndpoint
        {
            get
            {
                return new Endpoint
                {
                    BaseUri = ApiBaseUrl,
                    Resource = string.Format("/{0}/oauth2/token", Version)
                };
            }
        }

        public override string Name
        {
            get { return "dropbox"; }
        }

        protected override RestSharp.Portable.Authenticators.OAuth2.Models.UserInfo ParseUserInfo(string content)
        {
            throw new NotImplementedException();
        }

        protected override Endpoint UserInfoServiceEndpoint
        {
            get
            {
                return new Endpoint
                {
                    BaseUri = ApiBaseUrl,
                    Resource = string.Format("/{0}/account/info", Version)
                };
            }
        }
    }
}
