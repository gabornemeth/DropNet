using DropNet.Exceptions;
using DropNet.Models;
using System;
using System.Threading.Tasks;

namespace DropNet.Helpers
{
    public class ChunkedUploadHelper
    {
        private const long DefaultMaxRetries = 100;
        private readonly DropNetClient _client;
        private readonly Func<long, byte[]> _chunkNeeded;
        private readonly string _path;
        private readonly Action<ChunkedUploadProgress> _progress;
        private readonly bool _overwrite;
        private readonly string _parentRevision;
        private readonly long? _fileSize;
        private readonly long? _maxRetries;
        private long _chunksCompleted;
        private long _chunksFailed;
        private ChunkedUpload _lastChunkUploaded;

        public ChunkedUploadHelper(DropNetClient client, Func<long, byte[]> chunkNeeded, string path, Action<ChunkedUploadProgress> progress, bool overwrite, string parentRevision, long? fileSize, long? maxRetries)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            if (chunkNeeded == null)
            {
                throw new ArgumentNullException("chunkNeeded");
            }

            _client = client;
            _chunkNeeded = chunkNeeded;
            _path = path;
            _progress = progress;
            _overwrite = overwrite;
            _parentRevision = parentRevision;
            _fileSize = fileSize;
            _maxRetries = maxRetries;
            _lastChunkUploaded = new ChunkedUpload(); // initial chunk
        }

        async public Task<MetaData> StartAsync()
        {
            var firstChunk = _chunkNeeded.Invoke(0);
            var chunkLength = firstChunk.GetLength(0);
            if (chunkLength <= 0)
            {
                throw new DropboxException("Aborting chunked upload because chunkNeeded function returned no data on first call.");
            }

            UpdateProgress(0, null);
            var chunk = await _client.ChunkedUploadAsync(_lastChunkUploaded, firstChunk);
            return await OnChunkSuccess(chunk);
        }

        private void UpdateProgress(long offset, string uploadId)
        {
            if (_progress != null)
            {
                _progress.Invoke(new ChunkedUploadProgress(uploadId, _chunksCompleted, offset, _chunksFailed, _fileSize));
            }
        }

        async private Task<MetaData> OnChunkSuccess(ChunkedUpload chunkedUpload)
        {
            _chunksCompleted++;
            _lastChunkUploaded = chunkedUpload;
            UpdateProgress(chunkedUpload.Offset, chunkedUpload.UploadId);
            var offset = chunkedUpload.Offset;
            var nextChunk = _fileSize.GetValueOrDefault(long.MaxValue) > offset
                                ? _chunkNeeded.Invoke(offset)
                                : new byte[0];

            var chunkLength = nextChunk.GetLength(0);
            if (chunkLength > 0)
            {
                var chunk = await _client.ChunkedUploadAsync(chunkedUpload, nextChunk);
                await OnChunkSuccess(chunk);
            }

            return await _client.CommitChunkedUploadAsync(chunkedUpload, _path, _overwrite, _parentRevision);
        }
    }
}