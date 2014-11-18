using DropNet.Models;
using System.Threading.Tasks;

namespace DropNet
{
    public partial class DropNetClient
    {
        /// <summary>
        /// Gets AccountInfo
        /// </summary>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        async public Task<AccountInfo> GetAccountInfoAsync()
        {
            //This has to be here as Dropbox change their base URL between calls
            var request = _requestHelper.CreateAccountInfoRequest();
            return await Execute<AccountInfo>(ApiType.Base, request);
        }
    }
}