using DropNet.Exceptions;
using DropNet.Models;
using DropNet.Tests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DropNet.Tests
{
    [TestClass]
    public class FileTests
    {
        readonly DropNetClient _client;
        readonly Fixture _fixture;

        public FileTests()
        {
            _client = TestSettings.CreateClient();
            _fixture = new Fixture();
        }

        [TestMethod]
        async public Task Can_Get_MetaData_With_Special_Char_Async()
        {
            var localContent = _fixture.CreateAnonymous<string>();
            var content = Encoding.ASCII.GetBytes(localContent);
            await _client.UploadFileAsync("/Test", "test'.txt", content);
            var data = await _client.GetMetaDataAsync("/Test/test'.txt");
            Assert.IsNotNull(data);
        }

        [TestMethod]
        async public Task Can_Get_List_Of_Metadata_For_Search_String()
        {
            await _client.UploadIfNotExists("/Getting started.pdf");
            var results = await _client.SearchAsync("Getting");
            Assert.IsNotNull(results);
            Assert.IsTrue(results.Count > 0);
        }

        [TestMethod]
        async public Task Can_Upload_Chunked_File()
        {
            var localFile = new FileInfo(TestSettings.LocalDataFolder + "/Getting started.pdf");

            using (var fileStream = localFile.OpenRead())
            {
                byte[] buffer = new byte[1024 * 200]; // 200 KB buffer size
                var metaData = await _client.UploadChunkedFileAsync(TestSettings.TestFolder + "/Getting started chunked.pdf",
                    offset =>
                    {
                        if (fileStream.Length - fileStream.Position >= buffer.Length)
                        {
                            // there is enough data remaining in the stream to fill the buffer
                            fileStream.Read(buffer, 0, buffer.Length);
                            return buffer;
                        }
                        else
                        {
                            byte[] remainingData = new byte[fileStream.Length - fileStream.Position];
                            fileStream.Read(remainingData, 0, remainingData.Length);
                            return remainingData;
                        }

                    });
                Can_Upload_File_Async_Success(metaData);
            }
        }

        [TestMethod]
        async public Task Can_Upload_File_Async()
        {
            var localFile = new FileInfo(_fixture.CreateAnonymous<string>());
            try
            {
                var localContent = _fixture.CreateAnonymous<string>();

                File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
                Assert.IsTrue(File.Exists(localFile.FullName));
                byte[] content = File.ReadAllBytes(localFile.FullName);

                var data = await _client.UploadFileAsync("/Test", localFile.Name, content);
                Can_Upload_File_Async_Success(data);
            }
            finally
            {
                File.Delete(localFile.FullName);
            }
        }

        [TestMethod]
        async public Task Can_Upload_File_Async_International_Char()
        {
            var localFile = new FileInfo(_fixture.CreateAnonymous<string>());
            try
            {
                var localContent = _fixture.CreateAnonymous<string>();

                File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
                Assert.IsTrue(File.Exists(localFile.FullName));
                byte[] content = File.ReadAllBytes(localFile.FullName);

                var data = await _client.UploadFileAsync("/Test", "testПр1.txt", content);
                Can_Upload_File_Async_Success(data);
            }
            finally
            {
                File.Delete(localFile.FullName);
            }
        }

        [TestMethod]
        async public Task Can_Upload_File_Async_Streaming()
        {
            var localFile = new FileInfo(_fixture.CreateAnonymous<string>());
            try
            {
                var localContent = _fixture.CreateAnonymous<string>();

                File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
                Assert.IsTrue(File.Exists(localFile.FullName));
                byte[] content = File.ReadAllBytes(localFile.FullName);

                //var waitForUploadFinished = new ManualResetEvent(false);
                using (var fileStream = localFile.OpenRead())
                {
                    var metaData = await _client.UploadFileAsync("/Test", localFile.Name, fileStream);
                    Assert.IsNotNull(metaData);
                    Can_Upload_File_Async_Success(metaData);
                }
            }
            finally
            {
                File.Delete(localFile.FullName);
            }
        }

        private void Can_Upload_File_Async_Success(MetaData metadata)
        {
            Assert.IsNotNull(metadata);
        }

        [TestMethod]
        async public Task Can_Upload_Large_File_Async()
        {
            var localFile = new FileInfo(_fixture.CreateAnonymous<string>());
            try
            {
                var localContent = _fixture.CreateAnonymous<string>();

                for (int i = 0; i < 16; i++)
                {
                    localContent += localContent;
                }

                File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
                Assert.IsTrue(File.Exists(localFile.FullName));
                byte[] content = File.ReadAllBytes(localFile.FullName);

                var data = await _client.UploadFileAsync("/Test", localFile.Name, content);//, Can_Upload_Large_File_Async_Success, Can_Upload_Large_File_Async_Failure);
                Can_Upload_Large_File_Async_Success(data);
            }
            finally
            {
                File.Delete(localFile.FullName);
            }
        }

        private void Can_Upload_Large_File_Async_Success(MetaData metadata)
        {
            Assert.IsNotNull(metadata);
        }
        private void Can_Upload_Large_File_Async_Failure(DropboxException error)
        {
            Assert.IsTrue(false);
        }

        [TestMethod]
        async public Task Can_Get_Shares_Async()
        {
            await _client.UploadIfNotExists("/Getting Started.pdf");
            var response = await _client.GetShareAsync(TestSettings.TestFolder + "/Getting Started.pdf");
            Assert.IsNotNull(response);
        }

        [TestMethod]
        async public Task Can_Get_Upload_and_Get_Thumbnail_Async()
        {
            var metaData = await _client.GetMetaDataAsync("/Test/dropbox.jpg");
            if (metaData == null || metaData.Is_Deleted)
            {
                var contents = File.ReadAllBytes("TestData/dropbox.jpg");
                await _client.UploadFileAsync("/Test", "dropbox.jpg", contents);
            }
            var thumbnail = await _client.GetThumbnailAsync("/Test/dropbox.jpg");
            Can_Get_Thumbnail_Async_Success(thumbnail);
        }

        public void Can_Get_Thumbnail_Async_Success(byte[] rawBytes)
        {
            Assert.IsNotNull(rawBytes);
            //Save to disk for validation
            File.WriteAllBytes(@"C:\Temp\Test.png", rawBytes);
        }

        private void Can_Get_Thumbnail_Async_Failure(DropboxException error)
        {
            Assert.IsTrue(false);
        }

    }
}
