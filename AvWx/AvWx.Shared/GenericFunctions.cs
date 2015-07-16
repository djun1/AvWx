﻿using System;
using Windows.Web.Http;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;

static class GenericCodeClass
{
    private static TimeSpan LoopTimerInterval = new TimeSpan(0, 0, 0, 0, 500); //Loop timer interval in seconds
    private static TimeSpan DownloadTimerInterval = new TimeSpan(0, 15, 0); //Download time interval in minutes
    private static string HomeStationURL;
    private static string HomeStationString;
    private static bool IsHomeStationChanged = false;
    private static HttpClient Client;
    private static HttpResponseMessage Message;
    private static int DownloadPeriod = 3;
    public static List<string> ExistingFiles = new List<string>();
    public static bool IsLoopPaused = false;
    public static bool IsAppResuming = false;
    private static string SatelliteType;
    private static string HomeStationCode;
    private static string HomeProvince;
    private static bool IsCanadaSelected;
    private static string TypeCode;

    //Provide access to private property specifying Loop timer Interval
    public static TimeSpan LoopInterval
    {
        get { return LoopTimerInterval; }
        set { LoopTimerInterval = value; }
    }

    //Provide access to private property specifying Download timer Interval
    public static TimeSpan DownloadInterval
    {
        get { return DownloadTimerInterval; }
        set { DownloadTimerInterval = value; }
    }

    public static int FileDownloadPeriod
    {
        get { return DownloadPeriod; }
        set { DownloadPeriod = value; }
    }

    //Provide access to private property specifying whether home station has changed
    public static bool HomeStationChanged
    {
        get { return IsHomeStationChanged; }
        set { IsHomeStationChanged = value; }
    }

    //Provide access to private property specifying Home Weather Station
    public static string HomeStation
    {
        get { return HomeStationURL; }
        set
        {
            if (HomeStationURL != value)
                IsHomeStationChanged = true;

            HomeStationURL = value;
        }
    }

    public static string HomeStationName
    {
        get { return HomeStationString; }
        set { HomeStationString = value; }
    }

    //Provide access to private property specifying the type of satellite imagery
    public static string SatelliteTypeString
    {
        get { return SatelliteType; }
        set { SatelliteType = value; }
    }

    //Provide access to private property specifying the home station code
    public static string HomeStationCodeString
    {
        get { return HomeStationCode; }
        set { HomeStationCode = value; }
    }

    //Provide access to private property specifying the home province
    public static string HomeProvinceName
    {
        get { return HomeProvince; }
        set { HomeProvince = value; }
    }

    public static bool CanadaSelected
    {
        get { return IsCanadaSelected; }
        set { IsCanadaSelected = value; }
    }

    public static string TypeCodeString
    {
        get { return TypeCode; }
        set { TypeCode = value; }
    }

    //public static DateTime GetDateTimeFromFile(string Filename)
    //{
    //    DateTime LocalDateTime;

    //    string Time;
    //    string Year;
    //    string Day;
    //    string Month;

    //    if (!HomeProvinceName.Equals("Polar Imagery") && !HomeStationCodeString.Equals("NEPAC") && !HomeStationCodeString.Equals("WEST_CAN_USA"))
    //    {
    //        Time = Filename.Substring(8, 4);
    //        Year = Filename.Substring(0, 4);
    //        Day = Filename.Substring(4, 3);
    //        LocalDateTime = new DateTime(Convert.ToInt32(Year) - 1, 12, 31, Convert.ToInt32(Time.Substring(0, 2)), Convert.ToInt32(Time.Substring(2, 2)), 0);
    //        LocalDateTime = LocalDateTime.AddDays(Convert.ToDouble(Day));
    //    }
    //    else if (HomeStationCodeString.Equals("WEST_CAN_USA"))
    //    {
    //        Time = Filename.Substring(8, 4);
    //        Year = Filename.Substring(0, 4);
    //        Day = Filename.Substring(6, 2);
    //        Month = Filename.Substring(4, 2);
    //        LocalDateTime = new DateTime(Convert.ToInt32(Year), Convert.ToInt32(Month), Convert.ToInt32(Day), Convert.ToInt32(Time.Substring(0, 2)), Convert.ToInt32(Time.Substring(2, 2)), 0);
    //    }
    //    else
    //    {
    //        Year = DateTime.Now.Year.ToString();
    //        Day = DateTime.Now.Day.ToString();
    //        Month = DateTime.Now.Month.ToString();
    //        Time = DateTime.Now.Hour.ToString("D2") + DateTime.UtcNow.Minute.ToString("D2");
    //        LocalDateTime = new DateTime(Convert.ToInt32(Year), Convert.ToInt32(Month), Convert.ToInt32(Day), Convert.ToInt32(Time.Substring(0, 2)), Convert.ToInt32(Time.Substring(2, 2)), 0);
    //    }

    //    LocalDateTime = LocalDateTime.ToLocalTime();

    //    return LocalDateTime;
    //}

    public static async Task GetListOfLatestFiles(List<string> FileNames)
    {
        Uri URI;
        //string StartDateTimeString;
        //Regex RegExp;
        //string RegExpString;
        //DateTime CurrDateTime = DateTime.Now.ToUniversalTime();
        //DateTime StartOfYearDate = new DateTime(CurrDateTime.Year - 1, 12, 31);
        //DateTime StartDateTime;
        //TimeSpan NoOfDays = CurrDateTime.Subtract(StartOfYearDate);

        ExistingFiles.Clear();

        if (IsHomeStationChanged == false)
        {
            foreach (string str in FileNames)
            {
                ExistingFiles.Add(str);
            }
        }

        FileNames.Clear();

        if (Client == null)
            Client = new HttpClient();

        //StartDateTime = CurrDateTime.Subtract(new TimeSpan(DownloadPeriod, 0, 0));    //Subtract a time span equal to the download period from the Current Time

        //if (HomeStationCodeString.Equals("WEST_CAN_USA"))
        //{
        //    URI = new Uri("http://www.atmos.washington.edu/cgi-bin/list.cgi?ir4km");

        //    StartDateTimeString = StartDateTime.Year.ToString() + StartDateTime.Month.ToString("D2") + StartDateTime.Day.ToString("D2")
        //        + StartDateTime.Hour.ToString("D2") + StartDateTime.Minute.ToString("D2") + ".gif";

        //    RegExpString = "/[0-9]+.gif>";

        //}
        //else
        //{
        //    URI = new Uri(HomeStationURL);

        //    StartDateTimeString = StartDateTime.Year.ToString() + StartDateTime.Subtract(StartOfYearDate).Days.ToString("D3")
        //        + "_" + StartDateTime.Hour.ToString("D2") + StartDateTime.Minute.ToString("D2") + SatelliteType + ".jpg";

        //    RegExpString = ">[0-9]+_[0-9]+" + SatelliteType + ".jpg<";

        //}

        ////Normal NOAA Download begin
        //if (!HomeProvince.Equals("Polar Imagery") && !HomeStationCodeString.Equals("NEPAC") && !HomeProvince.Equals("Analysis Charts"))
        //{
        //    Client.DefaultRequestHeaders.IfModifiedSince = StartDateTime;
        //    var HttpClientTask = Client.GetAsync(URI);

        //    if (DownloadPeriod != 1)
        //        FileNames.Add(StartDateTimeString);
        //    RegExp = new Regex(RegExpString);

        //    try
        //    {
        //        Message = await HttpClientTask;
        //    }
        //    catch (Exception e)
        //    {
        //        if (DownloadPeriod != 1)
        //            FileNames.Remove(StartDateTimeString);
        //        return;
        //    }

        //    if (Message.IsSuccessStatusCode)
        //    {
        //        int MessageLength = Message.Content.ToString().Length;
        //        MatchCollection Matches = RegExp.Matches(Message.Content.ToString());//.Substring(MessageLength/2));
        //        int Location;

        //        if (Matches.Count > 0)
        //        {
        //            foreach (Match match in Matches)
        //            {
        //                if (match.Success)
        //                {
        //                    string tmp = match.ToString();	//The regular expression matches the "<" and ">" signs around the filename. These signs have to be removed before adding the filename to the list
        //                    FileNames.Add(tmp.Substring(1, tmp.Length - 2));
        //                }
        //            }
        //            FileNames.Sort();

        //            if (DownloadPeriod != 1)    //"Latest" option not selected
        //            {
        //                int MaxFiles = 6;
        //                Location = FileNames.IndexOf(StartDateTimeString);
        //                FileNames.RemoveRange(0, Location + 1);

        //                if (DownloadPeriod == 6)
        //                    MaxFiles = 12;
        //                if (FileNames.Count > MaxFiles) //Limit the maximum number of files displayed in the loop.
        //                {
        //                    //Remove every other file starting from the second oldest
        //                    int Index;
        //                    int NoOfFilesToRemove = FileNames.Count - MaxFiles;

        //                    for (Index = 1; Index < FileNames.Count && NoOfFilesToRemove > 0; Index += 1, NoOfFilesToRemove--)
        //                        FileNames.RemoveAt(Index);
        //                }
        //            }
        //            else
        //                FileNames.RemoveRange(0, FileNames.Count - 1);
        //        }
        //    }
        //    else
        //    {
        //        //return some sort of error code?
        //    }
        //}//Normal NOAA download end. Handle special cases begin
        //else
        //{
        //if (HomeProvince.Equals("Polar Imagery"))
        //{
        //    if (HomeStationCodeString.Equals("YUKON "))
        //    {
        //        FileNames.Add("hrpt_ykn_" + SatelliteTypeString + "_100.jpg");
        //    }
        //    else if (HomeStationCodeString.Equals("HUDSON_BAY"))
        //    {
        //        FileNames.Add("hrpt_hsb_" + SatelliteTypeString + "_100.jpg");
        //    }
        //    else
        //    {
        //        FileNames.Add("hrpt_wel_" + SatelliteTypeString + "_100.jpg");
        //    }
        //}

        if (HomeProvince.Equals("Analysis Charts"))
        {
            if (HomeStationCodeString.Equals("surface"))
            {
                FileNames.Add("LatestPrev-analsfc.png");
                FileNames.Add("Latest-analsfc.png");
            }
            if (HomeStationCodeString.Equals("850"))
            {
                FileNames.Add("LatestPrev-anal850.png");
                FileNames.Add("Latest-anal850.png");
            }
            if (HomeStationCodeString.Equals("700"))
            {
                FileNames.Add("LatestPrev-anal700.png");
                FileNames.Add("Latest-anal700.png");
            }
            if (HomeStationCodeString.Equals("500"))
            {
                FileNames.Add("LatestPrev-anal500.png");
                FileNames.Add("Latest-anal500.png");
            }
            if (HomeStationCodeString.Equals("250"))
            {
                FileNames.Add("LatestPrev-anal250.png");
                FileNames.Add("Latest-anal250.png");
            }
        }

        if (HomeProvince.Equals("Local Graphic Forecasts (West Coast)"))
        {
            if (HomeStationCodeString.Equals("north"))
            {
                FileNames.Add("Latest-lgfzvr43_0000.png");
                FileNames.Add("Latest-lgfzvr43_0300.png");
                FileNames.Add("Latest-lgfzvr43_0600.png");
            }
            if (HomeStationCodeString.Equals("central"))
            {
                FileNames.Add("Latest-lgfzvr42_0000.png");
                FileNames.Add("Latest-lgfzvr42_0300.png");
                FileNames.Add("Latest-lgfzvr42_0600.png");
            }
            if (HomeStationCodeString.Equals("south"))
            {
                FileNames.Add("Latest-lgfzvr41_0000.png");
                FileNames.Add("Latest-lgfzvr41_0300.png");
                FileNames.Add("Latest-lgfzvr41_0600.png");
            }
        }

        if (HomeProvince.Equals("Lightning Map"))
        {
            if (HomeStationCodeString.Equals("lightningARC"))
            {
                GenericCodeClass.GetWeatherDataURLs(FileNames, 6);
                return;
            }
            if (HomeStationCodeString.Equals("lightningATL"))
            {
                GenericCodeClass.GetWeatherDataURLs(FileNames, 6);
                return;
            }
            if (HomeStationCodeString.Equals("lightningONT"))
            {
                GenericCodeClass.GetWeatherDataURLs(FileNames, 6);
                return;
            }
            if (HomeStationCodeString.Equals("lightningPAC"))
            {
                GenericCodeClass.GetWeatherDataURLs(FileNames, 6);
                return;
            }
            if (HomeStationCodeString.Equals("lightningWRN"))
            {
                GenericCodeClass.GetWeatherDataURLs(FileNames, 6);
                return;
            }
            if (HomeStationCodeString.Equals("lightningQUE"))
            {
                GenericCodeClass.GetWeatherDataURLs(FileNames, 6);
                return;
            }
        }

        if (HomeProvince.Equals("Graphical Area Forecasts"))
        {
            if (HomeStationCodeString.Equals("Arctic"))
            {
                if (TypeCodeString.Equals("Clouds &amp; Weather"))
                {
                    FileNames.Add("Latest-gfacn37_cldwx_000.png");
                    //FileNames.Add("Latest-gfacn37_cldwx_006.png");
                    //FileNames.Add("Latest-gfacn37_cldwx_012.png");
                }
                //if (TypeCodeString.Equals("Icing, Turbulence &amp; Freezing Level"))
                //{
                //    FileNames.Add("Latest-gfacn37_turbc_000.png");
                //    FileNames.Add("Latest-gfacn37_turbc_006.png");
                //    FileNames.Add("Latest-gfacn37_turbc_012.png");
                //}
            }
            if (HomeStationCodeString.Equals("Atlantic"))
            {

            }
            if (HomeStationCodeString.Equals("Nunavut"))
            {

            }
            if (HomeStationCodeString.Equals("Ontario &amp; Quebec"))
            {

            }
            if (HomeStationCodeString.Equals("Pacific"))
            {

            }
            if (HomeStationCodeString.Equals("Prairies"))
            {

            }
            if (HomeStationCodeString.Equals("Yukon &amp; NWT"))
            {

            }
        }

        //else if (HomeStationCodeString.Equals("NEPAC"))
        //{
        //    int i = 1;

        //    switch (FileDownloadPeriod)
        //    {
        //        case 3:
        //            i = 5;
        //            break;
        //        case 6:
        //            i = 1;
        //            break;
        //        case 1:
        //            i = 10;
        //            break;
        //    }

        //    for (; i <= 10; i++)
        //        FileNames.Add(i.ToString() + ".jpg");
        //}
    }
    //Handle special cases end
    //}

    public static void GetWeatherDataURLs(List<string> FileNames, int NoOfFiles)
    {
        DateTime CurrDateTime = DateTime.Now.ToUniversalTime();
        int i;

        //No need to save previous files as that is done in the function GetLatestFiles()

        CurrDateTime = CurrDateTime.AddMinutes(-CurrDateTime.Minute % 10);

        for (i = 0; i < NoOfFiles; i++)
        {
            FileNames.Add(CurrDateTime.Year.ToString() + CurrDateTime.Month.ToString("D2") + CurrDateTime.Day.ToString("D2") + CurrDateTime.Hour.ToString("D2") + CurrDateTime.Minute.ToString("D2") + ".png");
            CurrDateTime = CurrDateTime.AddMinutes(-10);
        }

        FileNames.Reverse();
    }

    public static async Task<int> GetFileUsingHttp(string URL, StorageFolder Folder, string FileName)
    {
        var URI = new Uri(URL);
        StorageFile sampleFile;
        DateTime CurrDateTime = DateTime.Now.ToUniversalTime();

        if (Client == null)
            Client = new HttpClient();

        ////Set appropriate download periods for polar and non-polar imagery. Files that have been modified/added during this period will be
        ////downloaded.
        //if (HomeProvinceName.Equals("Polar Imagery"))
        //    Client.DefaultRequestHeaders.IfModifiedSince = CurrDateTime.Subtract(new TimeSpan(6, 0, 0));
        //else
        //    Client.DefaultRequestHeaders.IfModifiedSince = CurrDateTime.Subtract(new TimeSpan(DownloadPeriod, 0, 0));

        Message = await Client.GetAsync(URI);

        if (Message.IsSuccessStatusCode)
        {
            sampleFile = await Folder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);// this line throws an exception
            var FileBuffer = await Message.Content.ReadAsBufferAsync();
            await FileIO.WriteBufferAsync(sampleFile, FileBuffer);
            return 0; //Return code to show an image was successfully downloaded.
        }
        else
        {
            return -1; //Error code to show image was not downloaded successfully.
        }
    }

    public static bool FileExists(IReadOnlyList<StorageFile> FileList, string FileName)
    {
        int i;

        for (i = 0; i < FileList.Count; i++)
            if (FileName.Equals(FileList[i].Name))
                return true;

        return false;
    }

    public static async Task<BitmapImage> GetBitmapImage(StorageFolder ImageFolder, string FileName)
    {
        if (ImageFolder == null)
        {
            ImageFolder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
        }

        StorageFile ImageFile = await ImageFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
        BitmapImage Image;

        Image = await LoadBitmapImage(ImageFile);

        return Image;
    }

    private static async Task<BitmapImage> LoadBitmapImage(StorageFile file)
    {
        BitmapImage Image = new BitmapImage();
        FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);

        Image.SetSource(stream);

        return Image;

    }

    //do not delete
    //public static async Task<WriteableBitmap> GetWriteableBitmap(StorageFolder ImageFolder, string FileName)
    //{
    //    StorageFile ImageFile;
    //    WriteableBitmap ImageBitmap;

    //    if (ImageFolder == null)
    //    {
    //        ImageFolder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
    //    }

    //    ImageFile = await ImageFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
    //    ImageBitmap = await LoadWriteableBitmap(ImageFile);

    //    return ImageBitmap;
    //}

    //private static async Task<WriteableBitmap> LoadWriteableBitmap(StorageFile file)
    //{
    //    WriteableBitmap ImageBitmap;
    //    FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);

    //    ImageBitmap = new WriteableBitmap(720, 480);//Image.PixelWidth,Image.PixelHeight);
    //    ImageBitmap.SetSource(stream);

    //    return ImageBitmap;

    //}

    public static async Task DeleteFiles(StorageFolder ImageFolder, List<string> FilesToDelete, bool DeleteAllFiles)
    {
        StorageFile File;
        int i;

        if (ImageFolder == null)
            ImageFolder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);

        if (DeleteAllFiles)
        {
            //Get list of files currently in the local data folder
            var FileList = await ImageFolder.GetFilesAsync();
            for (i = 0; i < FileList.Count; i++)
            {
                File = await ImageFolder.GetFileAsync(FileList[i].Name);
                await File.DeleteAsync();
            }
        }
        else
        {
            for (i = 0; i < FilesToDelete.Count; i++)
            {
                File = await ImageFolder.GetFileAsync(FilesToDelete[i].ToString());
                await File.DeleteAsync();
            }
        }


    }

    public static void SaveAppData(bool SettingsChanged)
    {
        Windows.Storage.ApplicationDataContainer RoamingSettings =
                Windows.Storage.ApplicationData.Current.RoamingSettings;

        if (RoamingSettings != null)
        {
            if (SettingsChanged == true)
            {
                RoamingSettings.Values["SatelliteType"] = SatelliteType;
                RoamingSettings.Values["HomeStationCode"] = HomeStationCode;
                RoamingSettings.Values["HomeStationURL"] = HomeStationURL;
                RoamingSettings.Values["HomeStationString"] = HomeStationString;
                RoamingSettings.Values["HomeProvince"] = HomeProvince;
                RoamingSettings.Values["DownloadPeriod"] = DownloadPeriod;
                RoamingSettings.Values["LoopTimerInterval"] = LoopTimerInterval.Milliseconds;
                //RoamingSettings.Values["IsCanadaSelected"] = IsCanadaSelected;
            }

            RoamingSettings.Values["IsLoopPaused"] = GenericCodeClass.IsLoopPaused;
        }
    }

    public static void GetSavedAppData()
    {
        Windows.Storage.ApplicationDataContainer RoamingSettings =
                Windows.Storage.ApplicationData.Current.RoamingSettings;

        if (RoamingSettings != null)
        {
            try
            {
                HomeStationURL = RoamingSettings.Values["HomeStationURL"].ToString();
                LoopTimerInterval = new TimeSpan(0, 0, 0, 0, (int)RoamingSettings.Values["LoopTimerInterval"]);
                HomeStationString = RoamingSettings.Values["HomeStationString"].ToString();
                DownloadPeriod = (int)RoamingSettings.Values["DownloadPeriod"];
                SatelliteType = RoamingSettings.Values["SatelliteType"].ToString();
                HomeStationCode = RoamingSettings.Values["HomeStationCode"].ToString();
                HomeProvince = RoamingSettings.Values["HomeProvince"].ToString();
                IsLoopPaused = (bool)RoamingSettings.Values["IsLoopPaused"];
                //IsCanadaSelected = (bool)RoamingSettings.Values["IsCanadaSelected"];

            }
            catch (Exception e)
            {
                HomeStationURL = "https://flightplanning.navcanada.ca/Latest/anglais/produits/analyses/surface/";
                LoopTimerInterval = new TimeSpan(0, 0, 0, 0, 500);
                HomeStationString = "Surface";
                DownloadPeriod = 1;
                SatelliteType = "vis";
                HomeStationCode = "surface";
                HomeProvince = "Analysis Charts";
                //IsCanadaSelected = true;
            }
        }
    }

    public static bool IsError(string s)
    {
        return s.Equals("Error.png");
    }

}
