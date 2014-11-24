using DropNet.Models;
using RestSharp.Portable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DropNet
{
    public partial class DropNetClient
    {
        /// <summary>
        /// Gets MetaData for a File or Folder. For a folder this includes its contents. For a file, this includes details such as file size.
        /// </summary>
        /// <param name="path">The path of the file or folder</param>
        async public Task<MetaData> GetMetaDataAsync(string path)
        {
            if (!string.IsNullOrEmpty(path) && !path.StartsWith("/"))
            {
                path = "/" + path;
            }

            var request = _requestHelper.CreateMetadataRequest(path, client.Root);
            var data = await Execute<MetaData>(ApiType.Base, request);
            return data;

        }

        /// <summary>
        /// Gets MetaData for a File or Folder. For a folder this includes its contents. For a file, this includes details such as file size.
        /// Optional 'hash' param returns HTTP code 304	(Directory contents have not changed) if contents have not changed since the
        /// hash was retrieved on a previous call.
        /// </summary>
        /// <param name="path">The path of the file or folder</param>
        /// <param name="hash">hash - Optional. Listing return values include a hash representing the state of the directory's contents. If you provide this argument to the metadata call, you give the service an opportunity to respond with a "304 Not Modified" status code instead of a full (potentially very large) directory listing. This argument is ignored if the specified path is associated with a file or if list=false.</param>
        async public Task<MetaData> GetMetaDataAsync(string path, string hash)
        {
            if (path != "" && !path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateMetadataRequest(path, client.Root);
            request.AddParameter("hash", hash);

            return await Execute<MetaData>(ApiType.Base, request);
        }

        /// <summary>
        /// Restores a file path to a previous revision.
        /// </summary>
        /// <param name="rev">The revision of the file to restore.</param>
        /// <param name="path">The path to the file.</param>
        async public Task<MetaData> RestoreAsync(string rev, string path)
        {
            var request = _requestHelper.CreateRestoreRequest(rev, path, client.Root);
            return await Execute<MetaData>(ApiType.Base, request);
        }

        /// <summary>
        /// Gets list of metadata for search string
        /// </summary>
        /// <param name="searchString">The search string </param>
        public Task<List<MetaData>> SearchAsync(string searchString)
        {
            return SearchAsync(searchString, string.Empty);
        }

        /// <summary>
        /// Gets list of metadata for search string
        /// </summary>
        /// <param name="searchString">The search string </param>
        /// <param name="path">The path of the file or folder</param>
        async public Task<List<MetaData>> SearchAsync(string searchString, string path)
        {
            var request = _requestHelper.CreateSearchRequest(searchString, path, client.Root);

            return await Execute<List<MetaData>>(ApiType.Base, request);
        }


        /// <summary>
        /// Downloads a File from dropbox given the path
        /// </summary>
        /// <param name="path">The path of the file to download</param>
        async public Task<IRestResponse> GetFileAsync(string path)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateGetFileRequest(path, client.Root);

            return await Execute(ApiType.Content, request);
        }

        /// <summary>
        /// Downloads a part of a File from dropbox given the path
        /// </summary>
        /// <param name="path">The path of the file to download</param>
        /// <param name="startByte">The index of the first byte to get.</param>
        /// <param name="endByte">The index of the last byte to get.</param>
        /// <param name="rev">Revision of the file</param>
        async public Task<IRestResponse> GetFileAsync(string path, long startByte, long endByte, string rev)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateGetFileRequest(path, client.Root, startByte, endByte, rev);

            return await Execute(ApiType.Content, request);
        }


        /// <summary>
        /// Uploads a File to Dropbox given the raw data.
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="filename">The Name of the file to upload to dropbox</param>
        /// <param name="fileData">The file data</param>
        /// <param name="overwrite">Specify wether the file upload should replace an existing file</param>
        /// <param name="parentRevision">The revision of the file you're editing</param>
        async public Task<MetaData> UploadFileAsync(string path, string filename, byte[] fileData, bool overwrite = true, string parentRevision = null)
        {
            if (path != "" && !path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateUploadFileRequest(path, filename, fileData, client.Root, overwrite, parentRevision);

            return await Execute<MetaData>(ApiType.Content, request);
        }

        /// <summary>
        /// Uploads a File to Dropbox given the raw data.
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="filename">The Name of the file to upload to dropbox</param>
        /// <param name="fileStream">The file data</param>
        /// <param name="overwrite">Specify wether the file upload should replace an existing file</param>
        /// <param name="parentRevision">The revision of the file you're editing</param>
        async public Task<MetaData> UploadFileAsync(string path, string filename, Stream fileStream, bool overwrite = true, string parentRevision = null)
        {
            if (path != "" && !path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateUploadFileRequest(path, filename, fileStream, client.Root, overwrite, parentRevision);

            return await Execute<MetaData>(ApiType.Content, request);
        }

        /// <summary>
        /// Uploads a File to Dropbox in chunks that are assembled into a single file when finished.
        /// </summary>
        /// <param name="chunkNeeded">The callback function that returns a byte array given an offset</param>
        /// <param name="path">The full path of the file to upload to</param>
        /// <param name="progress">The optional callback Action that receives upload progress</param>
        /// <param name="overwrite">Specify wether the file upload should replace an existing file</param>
        /// <param name="parentRevision">The revision of the file you're editing</param>
        /// <param name="fileSize">The total size of the file if available</param>
        /// <param name="maxRetries">The number of times to retry uploading if a chunk fails, unlimited if null.</param>
        async public Task<MetaData> UploadChunkedFileAsync(string path, Func<long, byte[]> chunkNeeded, Action<ChunkedUploadProgress> progress = null, bool overwrite = true, string parentRevision = null, long? fileSize = null, long? maxRetries = null)
        {
            var chunkedUploader = new DropNet.Helpers.ChunkedUploadHelper(this, chunkNeeded, path, progress, overwrite, parentRevision, fileSize, maxRetries);
            return await chunkedUploader.StartAsync();
        }

        /// <summary>
        /// Add data to a chunked upload given a byte array.
        /// </summary>
        /// <param name="upload">A ChunkedUpload object received from the StartChunkedUpload method</param>
        /// <param name="fileData">The file data</param>
        async public Task<ChunkedUpload> ChunkedUploadAsync(ChunkedUpload upload, byte[] fileData)
        {
            var request = _requestHelper.CreateChunkedUploadRequest(upload, fileData);
            return await Execute<ChunkedUpload>(ApiType.Content, request);
        }

        /// <summary>
        /// Commit a completed chunked upload
        /// </summary>
        /// <param name="upload">A ChunkedUpload object received from the StartChunkedUpload method</param>
        /// <param name="path">The full path of the file to upload to</param>
        /// <param name="overwrite">Specify wether the file upload should replace an existing file</param>
        /// <param name="parentRevision">The revision of the file you're editing</param>
        async public Task<MetaData> CommitChunkedUploadAsync(ChunkedUpload upload, string path, bool overwrite = true, string parentRevision = null)
        {
            var request = _requestHelper.CreateCommitChunkedUploadRequest(upload, path, client.Root, overwrite, parentRevision);
            return await Execute<MetaData>(ApiType.Content, request);
        }

        /// <summary>
        /// Deletes the file or folder from dropbox with the given path
        /// </summary>
        /// <param name="path">The Path of the file or folder to delete.</param>
        async public Task<IRestResponse> DeleteAsync(string path)
        {
            if (path != "" && !path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateDeleteFileRequest(path, client.Root);

            return await Execute(ApiType.Base, request);
        }

        /// <summary>
        /// Copies a file or folder on Dropbox
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to copy</param>
        /// <param name="toPath">The path to where the file or folder is getting copied</param>
        async public Task<IRestResponse> CopyAsync(string fromPath, string toPath)
        {
            if (!fromPath.StartsWith("/")) fromPath = "/" + fromPath;
            if (!toPath.StartsWith("/")) toPath = "/" + toPath;

            var request = _requestHelper.CreateCopyFileRequest(fromPath, toPath, client.Root);

            return await Execute(ApiType.Base, request);
        }

        /// <summary>
        /// Copies a file or folder on Dropbox using a copy_ref as the source.
        /// </summary>
        /// <param name="fromCopyRef">Specifies a copy_ref generated from a previous /copy_ref call</param>
        /// <param name="toPath">The path to where the file or folder is getting copied</param>
        async public Task<IRestResponse> CopyFromCopyRefAsync(string fromCopyRef, string toPath)
        {
            if (!toPath.StartsWith("/")) toPath = "/" + toPath;

            var request = _requestHelper.CreateCopyFileFromCopyRefRequest(fromCopyRef, toPath, client.Root);
            return await Execute(ApiType.Base, request);
        }

        /// <summary>
        /// Moves a file or folder on Dropbox
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to move</param>
        /// <param name="toPath">The path to where the file or folder is getting moved</param>
        async public Task<IRestResponse> MoveAsync(string fromPath, string toPath)
        {
            if (!fromPath.StartsWith("/")) fromPath = "/" + fromPath;
            if (!toPath.StartsWith("/")) toPath = "/" + toPath;

            var request = _requestHelper.CreateMoveFileRequest(fromPath, toPath, client.Root);
            return await Execute(ApiType.Base, request);
        }

        /// <summary>
        /// Creates a folder on Dropbox
        /// </summary>
        /// <param name="path">The path to the folder to create</param>
        async public Task<MetaData> CreateFolderAsync(string path)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateCreateFolderRequest(path, client.Root);
            return await Execute<MetaData>(ApiType.Base, request);
        }

        /// <summary>
        /// Creates and returns a shareable link to files or folders.
        /// Note: Links created by the /shares API call expire after thirty days.
        /// </summary>
        /// <param name="path">The path</param>
        public Task<ShareResponse> GetShareAsync(string path)
        {
            return GetShareAsync(path, true);
        }

        /// <summary>
        /// Creates and returns a shareable link to files or folders.
        /// Note: Links created by the /shares API call expire after thirty days.
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="shortUrl">True to shorten the share url </param>
        async public Task<ShareResponse> GetShareAsync(string path, bool shortUrl)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateShareRequest(path, client.Root, shortUrl);

            return await Execute<ShareResponse>(ApiType.Base, request);
        }

        /// <summary>
        /// Returns a link directly to a file.
        /// Similar to /shares. The difference is that this bypasses the Dropbox webserver, used to provide a preview of the file, so that you can effectively stream the contents of your media.
        /// </summary>
        /// <param name="path">The path</param>
        async public Task<ShareResponse> GetMediaAsync(string path)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateMediaRequest(path, client.Root);
            return await Execute<ShareResponse>(ApiType.Base, request);
        }

        /// <summary>
        /// A long-poll endpoint to wait for changes on an account. In conjunction with /delta, this call gives you a low-latency way to monitor an account for file changes.
        /// </summary>
        /// <param name="cursor">The value returned from the prior call to GetDelta.</param>
        /// <param name="timeout">An optional integer indicating a timeout, in seconds.
        ///  The default value is 30 seconds, which is also the minimum allowed value. The maximum is 480 seconds.</param>
        async public Task<LongpollDeltaResult> GetLongpollDeltaAsync(string cursor, int timeout = 30)
        {
            var request = _requestHelper.CreateLongpollDeltaRequest(cursor, timeout);
            return await Execute<LongpollDeltaResult>(ApiType.Notify, request);
        }

        /// <summary>
        /// The beta delta function, gets updates for a given folder
        /// </summary>
        /// <param name="IKnowThisIsBetaOnly"></param>
        /// <param name="cursor">The value returned from the prior call to GetDelta or an empty string</param>
        async public Task<DeltaPage> GetDeltaAsync(bool IKnowThisIsBetaOnly, string cursor)
        {
            if (!IKnowThisIsBetaOnly)
                return null;

            var request = _requestHelper.CreateDeltaRequest(cursor, null, null, false);

            return await Execute<DeltaPage>(ApiType.Base, request);
        }

        /// <summary>
        /// The beta delta function, gets updates for a given folder
        /// </summary>
        /// <param name="IKnowThisIsBetaOnly"></param>
        /// <param name="cursor">The value returned from the prior call to GetDelta or an empty string</param>
        /// <param name="pathPrefix">If present, this parameter filters the response to only include entries at or under the specified path</param>
        /// <param name="locale">If present the metadata returned will have its size field translated based on the given locale</param>
        /// <param name="includeMediaInfo">If true, each file will include a photo_info dictionary for photos and a video_info dictionary for videos with additional media info. When include_media_info is specified, files will only appear in delta responses when the media info is ready. If you use the include_media_info parameter, you must continue to pass the same value on subsequent calls using the returned cursor.</param>
        async public Task<DeltaPage> GetDeltaAsync(bool IKnowThisIsBetaOnly, string cursor, string pathPrefix, string locale, bool includeMediaInfo)
        {
            if (!IKnowThisIsBetaOnly) return null;

            if (!pathPrefix.StartsWith("/")) pathPrefix = "/" + pathPrefix;

            var request = _requestHelper.CreateDeltaRequest(cursor, pathPrefix, locale, includeMediaInfo);

            return await Execute<DeltaPage>(ApiType.Base, request);
        }

        /// <summary>
        /// Gets the thumbnail of an image given its MetaData
        /// </summary>
        /// <param name="file">The MetaData</param>
        public Task<byte[]> GetThumbnailAsync(MetaData file)
        {
            return GetThumbnailAsync(file.Path, ThumbnailSize.Small);
        }

        /// <summary>
        /// Gets the thumbnail of an image given its MetaData
        /// </summary>
        /// <param name="file">The metadat file</param>
        /// <param name="size">Thumbnail size</param>
        public Task<byte[]> GetThumbnailAsync(MetaData file, ThumbnailSize size)
        {
            return GetThumbnailAsync(file.Path, size);
        }

        /// <summary>
        /// Gets the thumbnail of an image given its path
        /// </summary>
        /// <param name="path">The path</param>
        public Task<byte[]> GetThumbnailAsync(string path)
        {
            return GetThumbnailAsync(path, ThumbnailSize.Small);
        }

        /// <summary>
        /// Gets the thumbnail of an image given its path
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="size">Thumbnail size</param>
        async public Task<byte[]> GetThumbnailAsync(string path, ThumbnailSize size)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateThumbnailRequest(path, size, client.Root);

            var response = await Execute(ApiType.Content, request);
            return response.RawBytes;
        }

        /// <summary>
        /// Creates and returns a copy_ref to a file.
        /// 
        /// This reference string can be used to copy that file to another user's Dropbox by passing it in as the from_copy_ref parameter on /fileops/copy.
        /// </summary>
        /// <param name="path">The path</param>
        async public Task<CopyRefResponse> GetCopyRefAsync(string path)
        {
            if (!path.StartsWith("/")) path = "/" + path;

            var request = _requestHelper.CreateCopyRefRequest(path, client.Root);

            return await Execute<CopyRefResponse>(ApiType.Base, request);
        }

    }
}
