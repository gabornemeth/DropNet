using DropNet.Extensions;
using System.IO;
using System.Threading.Tasks;

namespace DropNet.Tests.Extensions
{
    public static class DropNetExtensions
    {
        async public static Task UploadIfNotExists(this DropNetClient client, string dest, string srcLocal = null)
        {
            // if no local source specified, use the same as target
            if (srcLocal == null)
                srcLocal = dest;
            // full path needed both locally and on the Dropbox
            dest = TestSettings.TestFolder + dest;
            srcLocal = TestSettings.LocalDataFolder + srcLocal;

            var metaData = await client.GetMetaDataAsync(dest);
            if (metaData == null || metaData.Is_Deleted)
            {
                var contents = File.ReadAllBytes(srcLocal);
                await client.UploadFileAsync(dest.RemoveLastDirectory(), Path.GetFileName(dest), contents);
            }
        }
    }
}
