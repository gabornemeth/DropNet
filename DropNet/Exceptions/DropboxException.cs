using RestSharp.Portable;
using System;
using System.Net;

namespace DropNet.Exceptions
{
    public class DropboxException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// The response of the error call (for Debugging use)
        /// </summary>
        public IRestResponse Response { get; private set; }

        public DropboxException()
        {
        }

        public DropboxException(string message)
            : base(message)
        {

        }

        public DropboxException(IRestResponse response, Exception innerException) : 
            base(response != null ? response.ToString() : "Uknown error", innerException)
        {
            if (response != null)
            {
                Response = response;
                StatusCode = response.StatusCode;
            }
        }

        public DropboxException(IRestResponse r)
        {
            if (r != null)
            {
                Response = r;
                StatusCode = r.StatusCode;
            }
        }

    }
}
