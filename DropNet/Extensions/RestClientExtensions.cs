/* Code below modified from a version taken from Laurent Kempé's blog
 * http://www.laurentkempe.com/post/Extending-existing-NET-API-to-support-asynchronous-operations.aspx
 */

using RestSharp.Portable;
using System.Threading.Tasks;

namespace DropNet.Extensions
{
    public static class RestClientExtensions
    {
        public static Task<IRestResponse<TResult>> ExecuteTask<TResult>(this IRestClient client, IRestRequest request) where TResult : new()
        {
#if WINDOWS_PHONE_APP
            //check for network connection
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                throw new DropboxException
                {
                    StatusCode = System.Net.HttpStatusCode.BadGateway
                };
                return;
            }
#endif
            return client.Execute<TResult>(request);
        }


        public static Task<IRestResponse> ExecuteTask(this IRestClient client, IRestRequest request)
        {
#if WINDOWS_PHONE_APP
            //check for network connection
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                throw new DropboxException
                {
                    StatusCode = System.Net.HttpStatusCode.BadGateway
                };
                return;
            }
#endif
            return client.Execute(request);
        }
    }
}