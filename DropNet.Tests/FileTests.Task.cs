using DropNet.Tests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;

namespace DropNet.Tests
{
    [TestClass]
    public class FileTaskTests
    {
        DropNetClient _client;
        readonly Fixture _fixture;

        public FileTaskTests()
        {
            _client = TestSettings.CreateClient();
            _fixture = new Fixture();
        }
        
        // TODO: there should be no need to call this in the beginning of every test method!
        public void Initialize()
        {
            var task = _client.UploadIfNotExists("/Getting'Started.pdf", "/Getting Started.pdf");
            task.Wait();
            task = _client.UploadIfNotExists("/Video.mp4", "/Video.mp4");
            task.Wait();
        }

        [TestMethod]
        public void Task_Get_MetaData()
        {
            Initialize();

            var path = TestSettings.TestFolder;
            var metaTask = _client.GetMetaDataAsync(path);

            metaTask.Wait();

            Assert.IsNotNull(metaTask.Result);
            Assert.IsNotNull(metaTask.Result.Contents);
            Assert.AreEqual(0, string.Compare(path, metaTask.Result.Path, true));
        }

        [TestMethod]
        public void Task_Get_MetaData_With_Special_Char()
        {
            Initialize();

            var path = TestSettings.TestFolder + "/Getting'Started.pdf";
            var metaTask = _client.GetMetaDataAsync(path);

            metaTask.Wait();

            Assert.IsNotNull(metaTask.Result);
            Assert.AreEqual(0, string.Compare(path, metaTask.Result.Path, true));
        }

        [TestMethod]
        public void Task_Can_Get_Media()
        {
            Initialize();

            var mediaTask = _client.GetMediaAsync(TestSettings.TestFolder + "/Video.mp4");

            mediaTask.Wait();

            Assert.IsNotNull(mediaTask.Result);
            Assert.IsNotNull(mediaTask.Result.Expires);
            Assert.IsNotNull(mediaTask.Result.Url);
        }

    }
}
