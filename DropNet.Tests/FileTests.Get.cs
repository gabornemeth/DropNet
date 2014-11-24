using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropNet.Tests.Extensions;

namespace DropNet.Tests
{
    [TestClass]
    public class GetFileTests
    {
        readonly DropNetClient _client;
        readonly Fixture _fixture;

        public GetFileTests()
        {
            _client = TestSettings.CreateClient();
            _fixture = new Fixture();
        }

        [TestMethod]
        async public Task Can_Get_File()
        {
            await _client.UploadIfNotExists("/Getting started.pdf");
            var response = await _client.GetFileAsync(TestSettings.TestFolder + "/Getting started.pdf");
            Assert.IsNotNull(response);
        }

    }
}
