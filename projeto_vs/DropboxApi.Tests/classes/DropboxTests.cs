using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Dropbox.Tests
{
    [TestClass()]
    public class DropboxTests
    {
        /*
        [TestMethod()]
        public void DropBoxBaseTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CloseTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateFolderTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FolderExistsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FileExistsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateSharedLinkTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetSharedLinkTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetTempLinkTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UploadTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DownloadTest()
        {
            Assert.Fail();
        }*/

        [TestMethod()]
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