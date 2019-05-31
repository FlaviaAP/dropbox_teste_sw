using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Dropbox.Tests
{
    [TestClass()]
    public class DropboxTests
    {
        DropBoxBase dropboxBase;

        //[TestMethod()]
        public void DropBoxBaseTest()
        {
            Assert.Fail();
        }

        //[TestMethod()]
        public void CloseTest()
        {
            Assert.Fail();
        }

        //[TestMethod()]
        public async Task CreateFolderTest()
        {
            Assert.Fail();
        }

        //[TestMethod()]
        public async Task FolderExistsTest()
        {
            Assert.Fail();
        }

        //[TestMethod()]
        public async Task FileExistsTest()
        {
            Assert.Fail();
        }

        //[TestMethod()]
        public async Task DeleteTest()
        {
            Assert.Fail();
        }

        //[TestMethod()]
        public async Task CreateSharedLinkTest()
        {
            try
            {
                dropboxBase = new DropBoxBase();
                var url = await dropboxBase.CreateSharedLink("dir1/math.jpg");
                dropboxBase.Close();

                Assert.AreNotEqual(null, url);
                Console.Write("url = {0}", url);
            }
            catch (Exception e)
            {
                dropboxBase.Close();
                Console.Write(e.ToString());
                throw e;
            }
        }

        [TestMethod()]
        public async Task GetSharedLinkExistTest()
        {
            try
            {
                var urlExpected = "https://www.dropbox.com/s/j02x06nvxvlec33/math.jpg?dl=0";
                dropboxBase = new DropBoxBase();
                var url = await dropboxBase.GetSharedLink("dir1/math.jpg");
                dropboxBase.Close();

                Assert.AreEqual(urlExpected, url);
            }
            catch (Exception e)
            {
                dropboxBase.Close();
                Console.Write(e.ToString());
                throw e;
            }
        }

        //[TestMethod()]
        public async Task GetSharedLinkNotExistTest()
        {
            try
            {
                dropboxBase = new DropBoxBase();
                var url = await dropboxBase.GetSharedLink("dir2/trab_teste_sw.pdf");
                Assert.AreEqual(null, url);
                dropboxBase.Close();
            }
            catch (Exception e)
            {
                dropboxBase.Close();
                Console.Write(e.ToString());
                throw e;
            }
        }

        //[TestMethod()]
        public async Task GetTempLinkTest()
        {
            Assert.Fail();
        }

        //[TestMethod()]
        public async Task UploadTest()
        {
            Assert.Fail();
        }

        //[TestMethod()]
        public async Task DownloadTest()
        {
            Assert.Fail();
        }

        //[TestMethod()]
        public async Task InitTestDirectory()
        {
            try
            {
                DropBoxBase dropboxBase = new DropBoxBase();

                await dropboxBase.CreateFolder("dir1");
                await dropboxBase.CreateFolder("dir2");

                var mathImgPath = "F:/Imagens/math.jpg";
                var bugDeadImgPath = "F:/Imagens/testcase.png";
                var especPdfPath = "C:/Users/Usuário/Downloads/Enunciado.pdf";

                await dropboxBase.Upload("dir1/math.jpg", mathImgPath);
                await dropboxBase.Upload("dir2/trab_teste_sw.pdf", especPdfPath);
                await dropboxBase.Upload(Path.GetFileName(bugDeadImgPath), bugDeadImgPath);

                dropboxBase.Close();
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
        }

        
    }
}