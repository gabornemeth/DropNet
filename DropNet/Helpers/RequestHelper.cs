using DropNet.Models;
using RestSharp.Portable;
using System;
using System.IO;
using System.Net.Http;

namespace DropNet.Helpers
{
    /// <summary>
    /// Helper class for creating DropNet RestSharp Requests
    /// </summary>
    public class RequestHelper
    {
        private readonly string _version;

        public RequestHelper(string version)
        {
            _version = version;
        }

        public RestRequest CreateMetadataRequest(string path, string root)
        {
            var request = new RestRequest("{version}/metadata/{root}{path}", HttpMethod.Get);
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);

            return request;
        }

        public RestRequest CreateVersionsRequest(string path, string root, int rev_limit)
        {
            var request = new RestRequest("{version}/revisions/{root}{path}", HttpMethod.Get);
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);
            request.AddParameter("rev_limit", rev_limit, ParameterType.UrlSegment);

            return request;
        }

        public RestRequest CreateShareRequest(string path, string root, bool shortUrl)
        {
            var request = new RestRequest("{version}/shares/{root}{path}", HttpMethod.Get);
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);
            request.AddParameter("short_url", shortUrl);

            return request;
        }

        public RestRequest CreateMediaRequest(string path, string root)
        {
            var request = new RestRequest("{version}/media/{root}{path}", HttpMethod.Get);
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);

            return request;
        }

        public RestRequest CreateCopyRefRequest(string path, string root)
        {
            var request = new RestRequest("{version}/copy_ref/{root}{path}", HttpMethod.Get);
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);

            return request;
        }

        public RestRequest CreateGetFileRequest(string path, string root)
        {
            var request = new RestRequest("{version}/files/{root}{path}", HttpMethod.Get);
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);

            return request;
        }

        public RestRequest CreateGetFileRequest(string path, string root, long startByte, long endByte, string rev)
        {
            var request = new RestRequest("{version}/files/{root}{path}", HttpMethod.Get);
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);
            request.AddParameter("rev", rev, ParameterType.UrlSegment);
            request.AddHeader("Range", "bytes=" + startByte + "-" + endByte);

            return request;
        }

        public RestRequest CreateUploadFileRequest(string path, string filename, byte[] fileData, string root, bool overwrite, string parent_revision)
        {
            var request = new RestRequest("{version}/files/{root}{path}", HttpMethod.Post);
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);
            //Need to add the "file" parameter with the file name
            // This isn't needed. Dropbox is particular about the ordering,
            // but the oauth sig only needs the filename, which we have in the OTHER parameter
            //request.AddParameter("file", filename);

            request.AddParameter("overwrite", overwrite);
            if (!String.IsNullOrEmpty(parent_revision))
            {
                request.AddParameter("parent_rev", parent_revision);
            }

            request.AddFile("file", fileData, filename);

            return request;
        }

        public RestRequest CreateUploadFilePutRequest(string path, string filename, byte[] fileData, string root, bool overwrite, string parent_revision)
        {
            var resource = "{version}/files_put/{root}{path}?file={file}&oauth_consumer_key={oauth_consumer_key}&oauth_nonce={oauth_nonce}" +
                "&oauth_token={oauth_token}&oauth_timestamp={oauth_timestamp}" +
                "&oauth_signature={oauth_signature}&oauth_signature_method={oauth_signature_method}&oauth_version={oauth_version}";
            var request = new RestRequest(resource, HttpMethod.Put);
            //Need to put the OAuth Parmeters in the Resource to get around them being put in the body
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);
            //Need to add the "file" parameter with the file name
            request.AddParameter("file", filename, ParameterType.UrlSegment);

            request.AddParameter("overwrite", overwrite);
            if (!String.IsNullOrEmpty(parent_revision))
            {
                request.AddParameter("parent_rev", parent_revision);
            }

            request.AddParameter("file", fileData, ParameterType.RequestBody);

            return request;
        }

        public RestRequest CreateUploadFileRequest(string path, string filename, Stream fileStream, string root, bool overwrite, string parent_revision)
        {
            var request = new RestRequest("{version}/files/{root}{path}", HttpMethod.Post);
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);
            //Need to add the "file" parameter with the file name
            // This isn't needed. Dropbox is particular about the ordering,
            // but the oauth sig only needs the filename, which we have in the OTHER parameter
            //request.AddParameter("file", filename);

            request.AddParameter("overwrite", overwrite);
            if (!String.IsNullOrEmpty(parent_revision))
            {
                request.AddParameter("parent_rev", parent_revision);
            }

            request.AddFile("file", fileStream, filename);

            return request;
        }

        public RestRequest CreateChunkedUploadRequest(ChunkedUpload upload, byte[] fileData)
        {
            var request = new RestRequest("{version}/chunked_upload", HttpMethod.Put);
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            if (upload.HasUploadId)
            {
                request.AddParameter("upload_id", upload.UploadId, ParameterType.UrlSegment);
                request.AddParameter("offset", upload.Offset, ParameterType.UrlSegment);
            }
            request.AddParameter("file", fileData, ParameterType.RequestBody);

            return request;
        }

        public RestRequest CreateCommitChunkedUploadRequest(ChunkedUpload upload, string path, string root, bool overwrite, string parent_revision)
        {
            var request = new RestRequest("{version}/commit_chunked_upload/{root}{path}", HttpMethod.Post);
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);

            request.AddParameter("overwrite", overwrite);
            request.AddParameter("upload_id", upload.UploadId);
            if (!String.IsNullOrEmpty(parent_revision))
            {
                request.AddParameter("parent_rev", parent_revision);
            }

            return request;
        }

        public RestRequest CreateDeleteFileRequest(string path, string root)
        {
            var request = new RestRequest("{version}/fileops/delete", HttpMethod.Get);
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("path", path);
            request.AddParameter("root", root);

            return request;
        }

        public RestRequest CreateCopyFileRequest(string fromPath, string toPath, string root)
        {
            var request = new RestRequest("{version}/fileops/copy", HttpMethod.Get);
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("from_path", fromPath);
            request.AddParameter("to_path", toPath);
            request.AddParameter("root", root);

            return request;
        }

        public RestRequest CreateCopyFileFromCopyRefRequest(string fromCopyRef, string toPath, string root)
        {
            var request = new RestRequest("{version}/fileops/copy", HttpMethod.Get);
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("from_copy_ref", fromCopyRef);
            request.AddParameter("to_path", toPath);
            request.AddParameter("root", root);

            return request;
        }

        public RestRequest CreateMoveFileRequest(string fromPath, string toPath, string root)
        {
            var request = new RestRequest("{version}/fileops/move", HttpMethod.Get);
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("from_path", fromPath);
            request.AddParameter("to_path", toPath);
            request.AddParameter("root", root);

            return request;
        }

        public RestRequest CreateLoginRequest(string apiKey, string email, string password)
        {
            var request = new RestRequest("{version}/token", HttpMethod.Get);
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("oauth_consumer_key", apiKey);

            request.AddParameter("email", email);

            request.AddParameter("password", password);

            return request;
        }

        public RestRequest CreateTokenRequest()
        {
            var request = new RestRequest("{version}/oauth/request_token", HttpMethod.Get);
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            return request;
        }

        public RestRequest CreateAccessTokenRequest()
        {
            var request = new RestRequest("{version}/oauth/access_token", HttpMethod.Get);
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            return request;
        }

        public RestRequest CreateAccessToken()
        {
            var request = new RestRequest("{version}/oauth/access_token", HttpMethod.Get);
            request.AddParameter("version", _version, ParameterType.UrlSegment);
            return request;
        }

        public RestRequest CreateNewAccountRequest(string apiKey, string email, string firstName, string lastName, string password)
        {
            var request = new RestRequest("{version}/account", HttpMethod.Post);
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("oauth_consumer_key", apiKey);

            request.AddParameter("email", email);
            request.AddParameter("first_name", firstName);
            request.AddParameter("last_name", lastName);
            request.AddParameter("password", password);

            return request;
        }

        public RestRequest CreateAccountInfoRequest()
        {
            var request = new RestRequest("{version}/account/info", HttpMethod.Get);
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            return request;
        }

        public RestRequest CreateCreateFolderRequest(string path, string root)
        {
            var request = new RestRequest("{version}/fileops/create_folder", HttpMethod.Get);
            request.AddParameter("version", _version, ParameterType.UrlSegment);

            request.AddParameter("path", path);
            request.AddParameter("root", root);

            return request;
        }

        internal RestRequest CreateLongpollDeltaRequest(string cursor, int timeout)
        {
            var request = new RestRequest("{version}/longpoll_delta", HttpMethod.Get);

            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("cursor", cursor);

            if (timeout < 30)
                timeout = 30;
            if (timeout > 480)
                timeout = 480;
            request.AddParameter("timeout", timeout);

            return request;
        }

        internal RestRequest CreateDeltaRequest(string cursor, string pathPrefix, string locale, bool includeMediaInfo)
        {
            var request = new RestRequest("{version}/delta", HttpMethod.Post);

            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("cursor", cursor);
            request.AddParameter("include_media_info", includeMediaInfo);

            if (!string.IsNullOrEmpty(pathPrefix))
            {
                request.AddParameter("path_prefix", pathPrefix);
            }

            if (!string.IsNullOrEmpty(locale))
            {
                request.AddParameter("locale", locale);
            }

            return request;
        }

        public RestRequest CreateThumbnailRequest(string path, ThumbnailSize size, string root)
        {
            var request = new RestRequest("{version}/thumbnails/{root}{path}", HttpMethod.Get);

            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);
            request.AddParameter("size", ThumbnailSizeString(size));

            return request;
        }

        private string ThumbnailSizeString(ThumbnailSize size)
        {
            switch (size)
            {
                case ThumbnailSize.Small:
                    return "small";
                case ThumbnailSize.Medium:
                    return "medium";
                case ThumbnailSize.Large:
                    return "large";
                case ThumbnailSize.ExtraLarge:
                    return "l";
                case ThumbnailSize.ExtraLarge2:
                    return "xl";
            }
            return "s";
        }

        public RestRequest CreateRestoreRequest(string rev, string path, string root)
        {
            var request = new RestRequest("{version}/restore/{root}{path}", HttpMethod.Post);

            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);
            request.AddParameter("rev", rev);

            return request;
        }

        public RestRequest CreateSearchRequest(string searchString, string path, string root)
        {
            var request = new RestRequest("{version}/search/{root}{path}", HttpMethod.Get);

            request.AddParameter("version", _version, ParameterType.UrlSegment);
            request.AddParameter("path", path, ParameterType.UrlSegment);
            request.AddParameter("root", root, ParameterType.UrlSegment);
            request.AddParameter("query", searchString);

            return request;
        }
    }

    internal static class StreamUtils
    {
        private const int STREAM_BUFFER_SIZE = 128 * 1024; // 128KB

        public static void CopyStream(Stream source, Stream target)
        { CopyStream(source, target, new byte[STREAM_BUFFER_SIZE]); }

        public static void CopyStream(Stream source, Stream target, byte[] buffer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (target == null) throw new ArgumentNullException("target");

            if (buffer == null) buffer = new byte[STREAM_BUFFER_SIZE];
            int bufferLength = buffer.Length;
            int bytesRead;
            while ((bytesRead = source.Read(buffer, 0, bufferLength)) > 0)
                target.Write(buffer, 0, bytesRead);
        }
    }
}
