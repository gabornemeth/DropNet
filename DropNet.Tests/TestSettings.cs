using RestSharp.Portable;
using RestSharp.Portable.Authenticators;
using RestSharp.Portable.Authenticators.OAuth2.Client;
using RestSharp.Portable.Authenticators.OAuth2.Configuration;
using RestSharp.Portable.Authenticators.OAuth2.Infrastructure;
using System.Configuration;

namespace DropNet.Tests
{
    public class TestAuthenticator : IAuthenticator
    {
        private string accessToken;

        public TestAuthenticator(string accessToken)
        {
            this.accessToken = accessToken;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            request.AddHeader("Authorization", "Bearer " + accessToken);
        }
    }

    public static class TestSettings
    {
        /// <summary>
        /// The folder where the sample test data is.
        /// </summary>
        public static string LocalDataFolder { get; private set; }
        /// <summary>
        /// This folder is used on your Dropbox for data storage during tests
        /// </summary>
        public static string TestFolder { get; private set; }

        static TestSettings()
        {
            LocalDataFolder = ConfigurationManager.AppSettings["LocalDataFolder"];
            TestFolder = ConfigurationManager.AppSettings["TestFolder"];
        }

        public static DropNetClient CreateClient()
        {
            var accessToken = ConfigurationManager.AppSettings["AccessToken"];
            var requestFactory = new RequestFactory();
            var config = new RuntimeClientConfiguration();
            var client = new DropboxClient(requestFactory, config);
            var authenticator = new TestAuthenticator(accessToken);
            return new DropNetClient(authenticator, client);
        }
    }
}
