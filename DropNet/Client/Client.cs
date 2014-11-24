using DropNet.Exceptions;
using DropNet.Helpers;
using RestSharp.Portable;
using RestSharp.Portable.Authenticators;
using RestSharp.Portable.Authenticators.OAuth2.Client;
using RestSharp.Portable.Authenticators.OAuth2.Infrastructure;
using RestSharp.Portable.Deserializers;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DropNet
{
    public partial class DropNetClient
    {
        private IAuthenticator authenticator;

        private readonly IRequestFactory requestFactory = new RequestFactory();

        private RestClient _restClient;
        private RestClient _restClientContent;
        private RestClient _restClientNotify;
        private RequestHelper _requestHelper;

        private IWebProxy _proxy;
        private DropboxClient client;

        /// <summary>
        /// Default Constructor for the DropboxClient
        /// </summary>
        /// <param name="authenticator">OAuth2 authenticator for the Dropbox requests</param>
        /// <param name="client">Settings of the Dropbox application</param>
        public DropNetClient(IAuthenticator authenticator, DropboxClient client, IWebProxy proxy = null)
        {
            this.authenticator = authenticator;
            this.client = client;
            _proxy = proxy;
            LoadClient();
        }

        /// <summary>
        /// Creates an instance of the DropNetClient given an API Key/Secret and an OAuth2 Access Token
        /// </summary>
        /// <param name="apiKey">The Api Key to use for the Dropbox Requests</param>
        /// <param name="appSecret">The Api Secret to use for the Dropbox Requests</param>
        /// <param name="accessToken">The OAuth2 access token</param>
        /// <param name="proxy">The proxy to use for web requests</param>
        //public DropNetClient(string apiKey, string appSecret, string accessToken, IWebProxy proxy)
        //{
        //    UserLogin = new UserLogin { Token = accessToken };
        //}

        /// <summary>
        /// Creates an instance of the DropNetClient given an API Key/Secret and an OAuth1 User Token/Secret
        /// </summary>
        /// <param name="apiKey">The Api Key to use for the Dropbox Requests</param>
        /// <param name="appSecret">The Api Secret to use for the Dropbox Requests</param>
        /// <param name="userToken">The OAuth1 User authentication token</param>
        /// <param name="userSecret">The OAuth1 Users matching secret</param>
        /// <param name="proxy">The proxy to use for web requests</param>
        //public DropNetClient(string apiKey, string appSecret, string userToken, string userSecret, IWebProxy proxy)
        //    : this(apiKey, appSecret, proxy)
        //{
        //    UserLogin = new UserLogin { Token = userToken, Secret = userSecret };
        //}

        private void LoadClient()
        {
            _restClient = new RestClient(DropboxClient.ApiBaseUrl);
            _restClient.Proxy = _proxy;

            _restClient.ClearHandlers();
            _restClient.Authenticator = authenticator;
            _restClient.AddHandler("*", new JsonDeserializer());

            _restClientContent = new RestClient(DropboxClient.ApiContentBaseUrl);
            _restClientContent.Authenticator = authenticator;
            _restClientContent.Proxy = _proxy;
            _restClientContent.ClearHandlers();
            _restClientContent.AddHandler("*", new JsonDeserializer());

            _restClientNotify = new RestClient(DropboxClient.ApiNotifyUrl);
            _restClientNotify.Authenticator = authenticator;
            _restClientNotify.Proxy = _proxy;
            _restClientNotify.ClearHandlers();
            _restClientNotify.AddHandler("*", new JsonDeserializer());

            _requestHelper = new RequestHelper(DropboxClient.Version);
        }

        async private Task<IRestResponse> Execute(ApiType apiType, IRestRequest request)
        {
            IRestResponse response = null;
            if (apiType == ApiType.Base)
            {
                //await Authenticate(_restClient, request);
                response = await _restClient.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new DropboxException(response);
                }
            }
            else if (apiType == ApiType.Content)
            {
                //await Authenticate(_restClientContent, request);
                response = await _restClientContent.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.PartialContent)
                {
                    throw new DropboxException(response);
                }
            }
            else
            {
                //await Authenticate(_restClientNotify, request);
                response = await _restClientNotify.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new DropboxException(response);
                }
            }

            return response;
        }

        async private Task<T> Execute<T>(ApiType apiType, IRestRequest request) where T : class
        {

            IRestResponse<T> response = null;
            if (apiType == ApiType.Base)
            {
                //await Authenticate(_restClient, request);
                response = await _restClient.Execute<T>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new DropboxException(response);
                }
            }
            else if (apiType == ApiType.Content)
            {
                try
                {
                    //await Authenticate(_restClientContent, request);
                    response = await _restClientContent.Execute<T>(request);

                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.PartialContent)
                    {
                        throw new DropboxException(response);
                    }
                }
                catch (Exception ex)
                {
                    object x = ex;
                }
            }
            else
            {
                //await Authenticate(_restClientNotify, request);
                response = await _restClientNotify.Execute<T>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new DropboxException(response);
                }
            }

            return response != null ? response.Data : null;
        }

        //async private Task Authenticate(IRestClient client, IRestRequest request)
        //{
        //    // workaround: AsyncAuthenticator causes deadlock when calls Wait on UI Context
        //    // If ConfigureAwait(false) is used, then Dropbox login page cannot be displayed because it does not run in UI syncronization context
        //    AsyncAuthenticator asyncAuthenticator = authenticator as AsyncAuthenticator;
        //    if (asyncAuthenticator != null)
        //        await asyncAuthenticator.Authenticate(_restClient, request);
        //    else
        //        authenticator.Authenticate(_restClient, request);
        //}

        enum ApiType
        {
            Base,
            Content,
            Notify
        }
    }
}
