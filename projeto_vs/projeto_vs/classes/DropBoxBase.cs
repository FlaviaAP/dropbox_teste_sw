using Dropbox.Api;
using Dropbox.Api.Files;
using Dropbox.Api.Sharing;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

public class DropBoxBase
{
    #region Variables  
    private DropboxClient DBClient;
    #endregion

    #region Constructor  
    public DropBoxBase()
    {
        AppName     = "TesteSW";
        AppKey      = "2mtr7y53tu0iupf";
        AppSecret   = "pv231kfuelaq3w1";
        AccessToken = "q2swAkRFJJ0AAAAAAAALRZumwvi4E3ITJt-8YiukN3xd2-CKgzkG04HzSY5klXP8";

        Authenticate();
    }
    #endregion

    #region Properties  
    private string AppName
    {
        get; set;
    }
    private string AppKey
    {
        get; set;
    }
    private string AppSecret
    {
        get; set;
    }
    private string AccessToken
    {
        get; set;
    }
    #endregion

    #region Init Methods  

    /// Validation method to verify that AppKey and AppSecret is not blank.  
    /// Mendatory to complete Authentication process successfully.  
    private bool CanAuthenticate()
    {
        try
        {
            if (AppKey == null)
            {
                throw new ArgumentNullException("AppKey");
            }
            if (AppSecret == null)
            {
                throw new ArgumentNullException("AppSecret");
            }
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private void Authenticate()
    {
        try
        {
            if (CanAuthenticate())
            {
                DropboxClientConfig CC = new DropboxClientConfig(AppName, 1);
                HttpClient HTC = new HttpClient();
                HTC.Timeout = TimeSpan.FromMinutes(10); // set timeout for each ghttp request to Dropbox API.  
                CC.HttpClient = HTC;
                DBClient = new DropboxClient(AccessToken, CC);

                Console.Write("Authenticate OK\n");
            }
        }
        catch (Exception e)
        {
            Console.Write("Exception on Authenticate\n");
            throw e;
        }
    }
    
    #endregion

    public void Close()
    {
        if (DBClient != null)
            DBClient.Dispose();
    }

    /// Force path to begin with '/', nedded for any dropbox's path
    private string CompletePath(string path)
    {
        return (path[0]=='/' ? path : "/"+path);
    }
    
    public async Task CreateFolder(string path)
    {
        try
        {
            if (AccessToken == null)
            {
                throw new Exception("AccessToken not generated !");
            }

            path = CompletePath(path);

            bool exist = await FolderExists(path);
            if (exist)
            {
                return;
            }

            var folderArg = new CreateFolderArg(path);
            await DBClient.Files.CreateFolderAsync(folderArg);

            Console.Write("Folder {0} created successfullly.\n", path);
        }
        catch (Exception e)
        {
            throw e;
        }
    }


    public async Task<bool> FolderExists(string path)
    {
        try
        {
            if (AccessToken == null)
            {
                throw new Exception("AccessToken not generated !");
            }

            path = CompletePath(path);
            await DBClient.Files.ListFolderAsync(path);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> FileExists(string path)
    {
        try
        {
            if (AccessToken == null)
            {
                throw new Exception("AccessToken not generated !");
            }

            path = CompletePath(path);
            await DBClient.Files.GetMetadataAsync(path);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public async Task<bool> Delete(string path)
    {
        try
        {
            if (AccessToken == null)
            {
                throw new Exception("AccessToken not generated !");
            }

            path = CompletePath(path);

            await DBClient.Files.DeleteAsync(path);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    #region Shared Link

    public async Task<string> CreateSharedLink(string path)
    {
        try
        {
            path = CompletePath(path);
            var settings = new SharedLinkSettings();
            var sharedLinkMetadata = await DBClient.Sharing.CreateSharedLinkWithSettingsAsync(path, settings);
            return sharedLinkMetadata.Url;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<string> GetSharedLink(string path)
    {
        try
        {
            path = CompletePath(path);
            //var settings = new SharedLinkSettings();
            var sharedLinkMetadata = await DBClient.Sharing.ListSharedLinksAsync(path);
            var links = sharedLinkMetadata.Links;

            string pathLower = path.ToLower();

            foreach (var link in links)
            {
                if (link.PathLower.Equals(pathLower))
                    return link.Url;
            }

            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    public async Task<string> GetTempLink(string path)
    {
        try
        {
            path = CompletePath(path);
            var response = await DBClient.Files.GetTemporaryLinkAsync(path);
            return response.Link;
        }
        catch (Exception)
        {
            return null;
        }
    }
    #endregion


    public async Task Upload(string dropboxFilePath, string sourceFilePath)
    {
        try
        {
            dropboxFilePath = CompletePath(dropboxFilePath);

            Console.Write("Upload of {0} ...", Path.GetFileName(sourceFilePath));

            using (var stream = new MemoryStream(File.ReadAllBytes(sourceFilePath)))
            {
                var response = await DBClient.Files.UploadAsync(dropboxFilePath, WriteMode.Overwrite.Instance, body: stream);
            }

            Console.Write("\nDone.\n");
        }
        catch (Exception e)
        {
            Console.Write("\nUpload Exception: dst=*{0}*, src=*{1}*\n", dropboxFilePath, sourceFilePath);//.
            throw e;
        }
    }

    public async Task Download(string dropboxFilePath, string dstLocalFilePath)
    {
        try
        {
            dropboxFilePath = CompletePath(dropboxFilePath);

            Console.Write("Download of {0} ...", Path.GetFileName(dstLocalFilePath));

            using (var response = await DBClient.Files.DownloadAsync(dropboxFilePath))
            {
                using (var fileStream = File.Create(dstLocalFilePath))
                {
                    (await response.GetContentAsStreamAsync()).CopyTo(fileStream);
                }
            }

            Console.Write("\nDone.\n");
        }
        catch (Exception e)
        {
            Console.Write("\nDownload Exception: src=*{0}*, dst=*{1}*\n", dropboxFilePath, dstLocalFilePath);
            throw e;
        }
    }
}