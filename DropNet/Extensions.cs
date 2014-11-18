#if !WINDOWS_PHONE_APP && !MONOTOUCH && !WINDOWS_APP
        /// <summary>
        /// Uploads a File to Dropbox from the local file system to the specified folder
        /// </summary>
        /// <param name="path">The path of the folder to upload to</param>
        /// <param name="localFile">The local file to upload</param>/// <param name="success">Success callback </param>
        /// <param name="failure">Failure callback </param>
        /// <param name="overwrite">Specify wether the file upload should replace an existing file</param>
        /// <param name="parentRevision">The revision of the file you're editing</param>
        public void UploadFileAsync(string path, FileInfo localFile, Action<MetaData> success, Action<DropboxException> failure, bool overwrite = true, string parentRevision = null)
        {
            //Get the file stream
            byte[] bytes;
            using (var fs = new FileStream(localFile.FullName, FileMode.Open, FileAccess.Read))
            {
                using (var br = new BinaryReader(fs))
                {
                    long numBytes = localFile.Length;
                    bytes = br.ReadBytes((int)numBytes);
                }
            }

            UploadFileAsync(path, localFile.Name, bytes, success, failure, overwrite, parentRevision);
        }
#endif
