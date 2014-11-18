using DropNet.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DropNet.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class FileTests_Sandbox
    {
        DropNetClient _client;
        Fixture fixture;

        public FileTests_Sandbox()
        {
            _client = new DropNetClient(TestVariables.ApiKey_Sandbox, TestVariables.ApiSecret_Sandbox);
            _client.UserLogin = new Models.UserLogin { Token = TestVariables.Token_Sandbox, Secret = TestVariables.Secret_Sandbox };
            _client.UseSandbox = true;

            fixture = new Fixture();
        }

        [TestMethod]
        async public void SANDBOX_Can_Get_MetaData_With_Special_Char()
        {
            var fileInfo = await _client.GetMetaDataAsync("/test'.txt");
            
            Assert.IsNotNull(fileInfo);
        }


        [TestMethod]
        async public void SANDBOX_Can_Get_File()
        {
            var fileInfo = await _client.GetFileAsync("/Sandbox.rtf");

            Assert.IsNotNull(fileInfo);
        }

        [TestMethod]
        async public void SANDBOX_Can_Get_File_Foreign_Language()
        {
            var rawBytes = await _client.GetFileAsync("/привет1.txt");

            Assert.IsNotNull(rawBytes);

            File.WriteAllBytes(@"C:\Temp\привет1.txt", rawBytes);
        }

        [TestMethod]
        async public void SANDBOX_Can_Get_File_And_Save()
        {
            var fileInfo = await _client.GetFile("/Sandbox.rtf");

            var writeStream = new FileStream("C:\\Temp\\Sandbox.rtf", FileMode.Create, FileAccess.Write);

            writeStream.Write(fileInfo, 0, fileInfo.Length);
            writeStream.Close();

            Assert.IsNotNull(fileInfo);
        }

        [TestMethod]
        public void SANDBOX_Can_Upload_File_PUT()
        {
            var localFile = new FileInfo(fixture.CreateAnonymous<string>() + ".txt");
            var localContent = fixture.CreateAnonymous<string>();

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = File.ReadAllBytes(localFile.FullName);

            var uploaded = _client.UploadFilePUT("/", localFile.Name, content);

            Assert.IsNotNull(uploaded);
            File.Delete(localFile.FullName);
        }

        [TestMethod]
        public void SANDBOX_Can_Upload_File()
        {
            var localFile = new FileInfo(fixture.CreateAnonymous<string>() + ".txt");
            var localContent = fixture.CreateAnonymous<string>();

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = File.ReadAllBytes(localFile.FullName);

            var uploaded = _client.UploadFile("/", localFile.Name, content);

            Assert.IsNotNull(uploaded);
            File.Delete(localFile.FullName);
        }

        [TestMethod]
        public void SANDBOX_Can_Upload_File_With_Special_Char()
        {
            var localFile = new FileInfo(fixture.CreateAnonymous<string>());
            var localContent = fixture.CreateAnonymous<string>();

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = File.ReadAllBytes(localFile.FullName);

            var uploaded = _client.UploadFile("/", "testfile's.txt", content);

            Assert.IsNotNull(uploaded);
            File.Delete(localFile.FullName);
        }

        [TestMethod]
        public void SANDBOX_Can_Upload_File_With_International_Char()
        {
            var localFile = new FileInfo(fixture.CreateAnonymous<string>());
            var localContent = fixture.CreateAnonymous<string>();

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = File.ReadAllBytes(localFile.FullName);

            var uploaded = _client.UploadFile("/", "testПр.txt", content);

            Assert.IsNotNull(uploaded);
            File.Delete(localFile.FullName);
        }

        [TestMethod]
        public void SANDBOX_Can_Upload_1MB_File()
        {
            var localFile = new FileInfo(fixture.CreateAnonymous<string>());
            var localContent = fixture.CreateAnonymous<string>();

            //Make a 1MB file...
            for (int i = 0; i < 15; i++)
            {
                localContent += localContent;
            }

            File.WriteAllText(localFile.FullName, localContent, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(localFile.FullName));
            byte[] content = File.ReadAllBytes(localFile.FullName);

            var uploaded = _client.UploadFile("/", localFile.Name, content);

            Assert.IsNotNull(uploaded);
            File.Delete(localFile.FullName);
        }

        [TestMethod]
        async public void SANDBOX_Can_Delete_File()
        {
            var deleted = await _client.DeleteAsync("/Test.txt");

            Assert.IsNotNull(deleted);
        }

        [TestMethod]
        async public void SANDBOX_Can_Get_MetaData()
        {
            var metaData = await _client.GetMetaDataAsync("/");

            Assert.IsNotNull(metaData);
            Assert.IsNotNull(metaData.Contents);
            Assert.AreEqual("app_folder", metaData.Root);
        }

        [TestMethod]
        public void SANDBOX_Can_Get_MetaData_Root()
        {
            var metaData = _client.GetMetaData();

            Assert.IsNotNull(metaData);
            Assert.IsNotNull(metaData.Contents);
        }

        [TestMethod]
        async public void SANDBOX_Can_Create_Folder()
        {
            MetaData metaData = null;
            var task = new Task(async () => { await _client.CreateFolderAsync(string.Format("TestFolder1{0:yyyyMMddhhmmss}", DateTime.Now),
                data => { metaData = data; },
                null); });
            task.RunSynchronously();

            Assert.IsNotNull(metaData);
        }

        [TestMethod]
        public void SANDBOX_Can_Shares()
        {
            _client.GetShare("/Sandbox.rtf");
        }

        [TestMethod]
        public void SANDBOX_Can_Get_Thumbnail()
        {
            var rawBytes = _client.GetThumbnail("/Test.png");

            Assert.IsNotNull(rawBytes);

            File.WriteAllBytes(@"C:\Temp\TestSandbox.png", rawBytes);
        }

        [TestMethod]
        async public void SANDBOX_Can_Get_Delta()
        {
            var deltaPage = await _client.GetDeltaAsync("", "", "", false);

            Assert.IsNotNull(deltaPage);

        }

    }
}
