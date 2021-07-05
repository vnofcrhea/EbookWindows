using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookWindows.Model;
using System.IO;
using MaterialDesignThemes.Wpf;
using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;
using Newtonsoft.Json;
using Google.Apis.Drive.v3;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Google.Apis.Download;
using System.ComponentModel;
using EbookWindows.Screen;
using System.IO.Compression;
using Syncfusion.Compression.Zip;
using System.Net;

namespace EbookWindows.ViewModels
{
    public class Settings_ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Settings _settings = new Settings();
        private readonly PaletteHelper _paletteHelper = new PaletteHelper();
        static string[] Scopes = { DriveService.Scope.DriveAppdata , @"https://www.googleapis.com/auth/userinfo.profile" };
        static string ApplicationName = "online-sync-data";
        readonly string startPath = @".\data\book";
        readonly string zipPath = @".\datasync.zip";
        readonly string extractPath = @".\data\book";
        string _Notification = "";
        UserCredential credential;
        DriveService driveService;
        public Settings_ViewModel()
        {
            try
            {
                if (File.Exists(App.Global.Directory_Folder + "\\data\\Settings.json"))
                {
                    using (StreamReader file = File.OpenText(App.Global.Directory_Folder + "\\data\\Settings.json"))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        Settings = (Settings)serializer.Deserialize(file, typeof(Settings));
                        if(_settings.IsLogged !=true)
                        {
                            _settings.IsLogged = false;
                            _settings.AccountName = "";
                        }    
                        else
                        {
                           Task.Run(()=>GetUserCredential());
                        }    
                    }
                }
                else
                {
                    _settings.BaseTheme = BaseTheme.Dark;
                    _settings.PrimaryColor = Color.FromRgb(0, 200, 255);
                    _settings.SecondaryColor = Color.FromRgb(50, 205, 50);
                    _settings.IsLogged = false;
                    _settings.AccountName = "";
                }
            }
            catch
            {
                _settings.BaseTheme = BaseTheme.Dark;
                _settings.PrimaryColor = Color.FromRgb(0, 200, 255);
                _settings.SecondaryColor = Color.FromRgb(50, 205, 50);
                _settings.IsLogged = false;
                _settings.AccountName = "";
            }
        }
        private Color old_Color;
        public Settings Settings
        {
            get { return _settings; }
            set { _settings = value;}
        }
        public string Notification
        {
            get { return _Notification; }
            set {
                if (value != _Notification)
                {

                    _Notification = value;
                    var handler = this.PropertyChanged;
                    if (handler != null)
                    {
                        handler(this,
                              new PropertyChangedEventArgs("Notification"));
                    }
                }
            }
        }
        public void ApplySetting()
        {
            ApplyBaseTheme();
            ApplyPrimaryColor();
            ApplySecondaryColor();
            //(App.Current.Resources.MergedDictionaries[0] as CustomColorTheme).BaseTheme = _settings.BaseTheme;
            //(App.Current.Resources.MergedDictionaries[0] as CustomColorTheme).PrimaryColor = _settings.PrimaryColor;
            //(App.Current.Resources.MergedDictionaries[0] as CustomColorTheme).SecondaryColor = _settings.SecondaryColor;
        }
        public void SaveSetting()
        {
            var path_data = App.Global.Directory_Folder + "\\data";
            #region create path. 
            if (!Directory.Exists(path_data))
            {
                Directory.CreateDirectory(path_data);
            }
            #endregion
            File.WriteAllText(path_data + "\\" + "Settings.json", JsonConvert.SerializeObject(_settings));
            //Save new settings to folder. 
        }
        public BaseTheme BaseTheme
        {
            get { return _settings.BaseTheme; }
            set { if (_settings.BaseTheme != value) { _settings.BaseTheme = value; ApplyBaseTheme(); } }
        }
        public Color PrimaryColor
        {
            get { return _settings.PrimaryColor; }
            set { if (_settings.PrimaryColor != value) { old_Color = _settings.PrimaryColor; _settings.PrimaryColor = value; ApplyPrimaryColor(); } }
        }
        public Color SecondaryColor
        {
            get { return _settings.SecondaryColor; }
            set { if (_settings.SecondaryColor != value) { _settings.SecondaryColor = value; ApplySecondaryColor(); } }
        }
        public bool IsDarkTheme
        {
            get { if (_settings.BaseTheme == BaseTheme.Dark) return true; else return false; }
            set
            {
                if (value) BaseTheme = BaseTheme.Dark; else BaseTheme = BaseTheme.Light;
                var handler = this.PropertyChanged;
                if (handler != null)
                {
                    handler(this,
                          new PropertyChangedEventArgs("IsDarkTheme"));
                }

            }
        }
        public bool IsLogged
        {
            get { return _settings.IsLogged; }
            set
            {
                if (value != _settings.IsLogged)
                {

                    _settings.IsLogged = value;
                    var handler = this.PropertyChanged;
                    if (handler != null)
                    {
                        handler(this,
                              new PropertyChangedEventArgs("IsLogged"));
                    }
                }
            }
        }
        public string AccountName
        {
            get { return _settings.AccountName; }
            set { _settings.AccountName = value; }
        }

        private void ApplyBaseTheme()
        {
            if (_settings.BaseTheme == BaseTheme.Light)
            {
                PrimaryColor = Color.FromRgb(65, 69, 21);
            }
            else
            {
                PrimaryColor = Color.FromRgb(0, 200, 255);
            }
            ITheme theme = _paletteHelper.GetTheme();
            theme.SetBaseTheme(_settings.BaseTheme.GetBaseTheme());
            if (_settings.BaseTheme == BaseTheme.Light)
                theme.Paper = Color.FromRgb(246, 244, 236);
            _paletteHelper.SetTheme(theme);
                
        }
        private void ApplyPrimaryColor()
        {
            ITheme theme = _paletteHelper.GetTheme();
            theme.PrimaryLight = new ColorPair(_settings.PrimaryColor.Lighten());
            theme.PrimaryMid = new ColorPair(_settings.PrimaryColor);
            theme.PrimaryDark = new ColorPair(_settings.PrimaryColor.Darken());
            _paletteHelper.SetTheme(theme);
        }
        private void ApplySecondaryColor()
        {
            ITheme theme = _paletteHelper.GetTheme();
            theme.PrimaryLight = new ColorPair(_settings.PrimaryColor.Lighten());
            theme.PrimaryMid = new ColorPair(_settings.PrimaryColor);
            theme.PrimaryDark = new ColorPair(_settings.PrimaryColor.Darken());
            _paletteHelper.SetTheme(theme);
        }



        public async void GetUserCredential()
        {

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                var credentialx = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    new CancellationTokenSource().Token,
                    new FileDataStore(credPath, true));
                if (credentialx.Token.IsExpired(Google.Apis.Util.SystemClock.Default))
                {
                    try
                    {
                        var refreshResult = credentialx.RefreshTokenAsync(CancellationToken.None).Result;
                        credential = credentialx;
                    }
                    catch
                    {
                        IsLogged = false;
                        return;
                    }
                }
                else
                {
                    credential = credentialx;
                }    
                var di = new DirectoryInfo(@".\token.json");
                if ((di.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    //Add Hidden flag    
                    di.Attributes |= FileAttributes.Hidden;
                }
                //var paragraph = new Paragraph();
                //paragraph.Inlines.Add(new Run(string.Format("Login Successed! Credential file saved to: " + credPath)));
                //content.Document.Blocks.Add(paragraph);
                
                driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                using (var web = new WebClient())
                {
                    var json = web.DownloadString(@"https://www.googleapis.com/oauth2/v2/userinfo?alt=json&access_token=" + credential.Token.AccessToken);
                    var content = JsonConvert.DeserializeObject<GoogleUserProfile>(json);
                    _settings.AccountName = content.name;
                }
                IsLogged = true;
            }

        }

        public void RevokeToken()
        {
            try
            {
                credential.RevokeTokenAsync(CancellationToken.None);
                IsLogged = false;
            }
            catch
            {
                IsLogged = false;
            }
        }

        public void UploadSyncData()
        {

            IList<Google.Apis.Drive.v3.Data.File> listfile;
            if (File.Exists(zipPath))
                File.Delete(zipPath);
            ZipFile.CreateFromDirectory(startPath, zipPath);
            var listRequest = driveService.Files.List();
            listRequest.Spaces = "appDataFolder";
            listRequest.Fields = "nextPageToken, files(id, name)";
            try
            {
                listfile = listRequest.Execute().Files;
            }
            catch
            {
                if (File.Exists(zipPath))
                    File.Delete(zipPath);
                return;
            }
            //string path = @"C:\Users\vnofc\source\repos\API-DRIVE-TEST\API-DRIVE-TEST\bin\Debug\netcoreapp3.1\abc.json";
            Google.Apis.Drive.v3.Data.File x = new Google.Apis.Drive.v3.Data.File();
            x.Name = System.IO.Path.GetFileName(zipPath);
            x.MimeType = GetMimeType(zipPath);
            x.Parents = new List<string>
                {
                    "appDataFolder"
                };
            var stream1 = new System.IO.FileStream(zipPath,
                    System.IO.FileMode.Open, System.IO.FileAccess.Read);
            //FilesResource.CreateMediaUpload request;
            try
            {
                if (listfile.Count != 0)
                {


                    //Delete and Upload new files to Google Drive
                    foreach (var item in listfile)
                    {
                        var deleteRequest = driveService.Files.Delete(item.Id);
                        deleteRequest.Execute();
                    }
                    var createRequest = driveService.Files.Create(x, stream1, GetMimeType(zipPath));
                    createRequest.Upload();
                    //var Response = createRequest.ResponseBody;

                }
                else
                {
                    // Create and Upload
                    var createRequest = driveService.Files.Create(x, stream1, GetMimeType(zipPath));
                    createRequest.Upload();
                    //var Response = createRequest.ResponseBody; for Debug
                }

                stream1.Dispose();
                File.Delete(zipPath);
            }
            catch
            {
                stream1.Dispose();
                File.Delete(zipPath);
                Notification = "Upload Failed! Check your connection and try again.";
            }
            Notification = "Upload Sucessful!";
        }


        public void DownLoadSyncData()
        {
            IList<Google.Apis.Drive.v3.Data.File> listfile;
            var listRequest = driveService.Files.List();
            listRequest.Spaces = "appDataFolder";
            listRequest.Fields = "nextPageToken, files(id, name)";
            listfile = listRequest.Execute().Files;

            if (listfile.Count == 0)
            {
                return;
            }
            else
            {
                if (File.Exists(zipPath))
                    File.Delete(zipPath);
                var fileId = listfile[0].Id;
                var downloadRequest = driveService.Files.Get(fileId);
                using (FileStream fs = new FileStream(listfile[0].Name, FileMode.Create))
                {
                    downloadRequest.MediaDownloader.ProgressChanged += Download_ProgressChanged;
                    downloadRequest.Download(fs);
                }
                //After
                if (!Directory.Exists(startPath))
                {
                    Directory.CreateDirectory(startPath);
                }
                //ZipFile.ExtractToDirectory(zipPath, startPath);
                using (System.IO.Compression.ZipArchive zipLoader = ZipFile.Open(zipPath, ZipArchiveMode.Read))
                {
                    foreach (var entry in zipLoader.Entries)
                    {
                        string path = Path.Combine(startPath, entry.FullName);
                        if (!Directory.Exists(Path.GetDirectoryName(path)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(path));
                        }
                        entry.ExtractToFile(path, true);
                    }
                    zipLoader.Dispose();
                }
                File.Delete(zipPath);
                App.Global.Book_Short_ViewModel.LoadListBookShort();
                (App.Current.MainWindow as WindowScreen).LoadTreeViewList();
                (App.Current.MainWindow as WindowScreen).LoadShelf();
            }
            Notification = "Download and Sync Sucessful!";

        }
        static void Download_ProgressChanged(IDownloadProgress progress)
        {
            switch (progress.Status)
            {
                case DownloadStatus.Downloading:
                    {
                        Console.WriteLine(progress.BytesDownloaded);
                        break;
                    }
                case DownloadStatus.Completed:
                    {
                        Console.WriteLine("Download complete.");

                        // Hoàn thành việc tải file xuống MemoryStream thì thực hiện việc chuyển MemoryStream ra FileStream thực tế

                        break;
                    }
                case DownloadStatus.Failed:
                    {
                        Console.WriteLine("Download failed.");
                        break;
                    }
            }
        }
        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
        public IList<Google.Apis.Drive.v3.Data.File> FileInFolder(string FileName, string FolderId)
        {
            var listRequest = driveService.Files.List();
            listRequest.Q = "name='" + FileName + "' and parents in '" + FolderId + "'";
            listRequest.Fields = "files(id,parents)";
            return listRequest.Execute().Files;
        }


    }
}
