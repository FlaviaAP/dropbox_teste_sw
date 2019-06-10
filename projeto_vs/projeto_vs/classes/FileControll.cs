using System.Threading.Tasks;
using System;
using System.IO;

public class FileControll
{
    private static DropBoxBase dropBoxBase;
    private static bool loaded = false;

    #region Properties

    public static bool Loaded {
        get { return loaded; }
    }

    #endregion

    void Start()
    {
        if (loaded)
            return;

        try
        {
            dropBoxBase = new DropBoxBase();

            loaded = true;
        }
        catch (Exception)
        {
            loaded = false;
        }
    }

    void OnApplicationQuit()
    {
        if (dropBoxBase != null)
            dropBoxBase.Close();
    }

    public static class Images
    {
        public const string SCHOOL_IMG_FILENAME = "logo";
        public static readonly string[] IMG_EXTENSIONS = { ".png", ".jpg" };
    }

    public static class Cloud
    {
        public static async Task Delete(string path)
        {
            await dropBoxBase.Delete(path);
        }

        public static async Task<bool> DirectoryExists(string path)
        {
            return await dropBoxBase.FolderExists(path);
        }

        public static async Task CreateDirectory(string path)
        {
            await dropBoxBase.CreateFolder(path);
        }

        public static async Task<bool> FileExists(string path)
        {
            return await dropBoxBase.FileExists(path);
        }

        #region Upload/Download

        public static async Task UploadFile(string cloudFilePath, string localSourceFilePath)
        {
            await dropBoxBase.Upload(cloudFilePath, localSourceFilePath);
        }

        public static async Task DownloadFile(string cloudFilePath, string localFilePath)
        {
            await dropBoxBase.Download(cloudFilePath, localFilePath);
        }
        
        #endregion
          
        #region Directories

        private const string
            SCHOOL_DIR_PREFIX = "Escola_",
            CLASSNUMBER_DIR_PREFIX = "Turma_"
        ;

        public static string SchoolDirPath(string schoolName)
        {
            return SCHOOL_DIR_PREFIX + schoolName;
        }

        public static string ClassDirPath(int classNumber, string schoolName)
        {
            return string.Format("{0}-{1}{2}", SchoolDirPath(schoolName), CLASSNUMBER_DIR_PREFIX, classNumber);
        }

        public static async Task CreateSchoolDirectory(string schoolName)
        {
            await CreateDirectory(SchoolDirPath(schoolName));
        }

        public static async Task CreateClassDirectory(int classNumber, string schoolName)
        {
            await CreateDirectory(ClassDirPath(classNumber, schoolName));
        }

        public static async Task DeleteSchoolDir(string schoolName)
        {
            try
            {
                string dirPath = SchoolDirPath(schoolName);
                await Delete(dirPath);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static async void DeleteClassDir(int classNumber, string schoolName)
        {
            try
            {
                string dirPath = ClassDirPath(classNumber, schoolName);
                await Delete(dirPath);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region School Logo

        public static async Task<string> GetSchoolLogoPath(string schoolName)
        {
            string dirPath = SchoolDirPath(schoolName);
            string cloudLogoPath = dirPath + "/" + Images.SCHOOL_IMG_FILENAME;
            string completePath = null;

            int i = 0;
            bool found = false;
            while (!found && i < Images.IMG_EXTENSIONS.Length)
            {
                string ext = Images.IMG_EXTENSIONS[i];
                completePath = cloudLogoPath + Images.IMG_EXTENSIONS[i];
                found = await dropBoxBase.FileExists(completePath);
                i++;
            }

            if (found)
            {
                return completePath;
            }

            return null;
        }

        public static async Task<bool> SchoolHasLogoFile(string schoolName)
        {
            string path = await GetSchoolLogoPath(schoolName);
            return await FileExists(path);
        }

        public static async Task<string> CreateSchoolDirAndUploadLogoImage(string schoolName, string logoExternalFullPath)
        {
            try
            {
                string dirPath = SchoolDirPath(schoolName);
                await CreateDirectory(dirPath);

                // upload logo image file in dir created now
                string cloudFilePath = dirPath + "/" + Images.SCHOOL_IMG_FILENAME + Path.GetExtension(logoExternalFullPath);
                await dropBoxBase.Upload(cloudFilePath, logoExternalFullPath);
                var url = await dropBoxBase.CreateSharedLink(cloudFilePath);
                return url;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static async Task<string> UpdateLogoFileAndGetSharedLink(string schoolName, string absoluteLocalFilePath)
        {
            string oldFilePath = await GetSchoolLogoPath(schoolName);
            string oldExtension = Path.GetExtension(oldFilePath);

            string newExtension = Path.GetExtension(absoluteLocalFilePath);
            string newFileName = Images.SCHOOL_IMG_FILENAME + newExtension;

            string cloudFilePath = SchoolDirPath(schoolName) + "/" + newFileName;

            await dropBoxBase.Upload(cloudFilePath, absoluteLocalFilePath);
            var url = await dropBoxBase.CreateSharedLink(cloudFilePath);

            if (!newExtension.Equals(oldExtension))
            {
                await Delete(oldFilePath);
            }

            return url;
        }
        #endregion

        #region Shared Link

        public static async Task<string> GetSchoolLogoSharedLink(string schoolName)
        {
            string remotePath = await GetSchoolLogoPath(schoolName);
            return await dropBoxBase.GetSharedLink(remotePath);
        }

        #endregion
    }

}

