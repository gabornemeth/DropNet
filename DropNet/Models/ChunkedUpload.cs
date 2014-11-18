using Newtonsoft.Json;
using System;

namespace DropNet.Models
{
    public class ChunkedUpload
    {
        [JsonProperty("upload_id")]
        public string UploadId { get; set; }
        public long Offset { get; set; }
        public string Expires { get; set; }
        
        public DateTime ExpiresDate
        {
            get
            {
                return Expires == null ? DateTime.MinValue : DateTime.Parse(Expires);
            }
        }

        public bool HasUploadId
        {
            get
            {
                return !string.IsNullOrEmpty(UploadId);
            }
        }
    }
}
