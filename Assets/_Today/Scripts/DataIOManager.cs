using Newtonsoft.Json;
using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataIOManager : MonoBehaviour
{
    private static DataIOManager instnace = null;
    public static DataIOManager Instance
    {
        get
        {
            QDebug.IsNull(instnace, "DataIOManaget Instance", null, QDebug.Severity.OhGodWhy);

            return instnace;
        }
    }

    [SerializeField]
    private string settingsFolderName = "Settings";

    [SerializeField]
    private string settingsFileName = "Settings.config";

    [SerializeField]
    private string dataFolderName = "Today Data";

    [SerializeField, ReadOnlyField]
    private string dataRootPath = "";




    void Awake()
    {
        if (instnace == null)
        {
            instnace = this;
        }
        else
        {
            Debug.LogError("Multiple Instances of DataIOManager Singleton");
            Destroy(this);
        }
    }

    void Start()
    {
        dataRootPath = GetSettingsData().dataRootPath;

        if (dataRootPath == null || dataRootPath == "")
        {
            dataRootPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
    }


    public SettingsData GetSettingsData()
    {
        string path = ValidateSettingsFile();

        SettingsData data;

        data = JsonConvert.DeserializeObject<SettingsData>(File.ReadAllText(path));

        if (data == null)
        {
            data = new SettingsData();
            SetSettingsData(data);
        }

        return data;
    }
    public void SetSettingsData(SettingsData data)
    {
        File.WriteAllText(ValidateSettingsFile(), JsonConvert.SerializeObject(data, Formatting.Indented));
    }
    private string ValidateSettingsDirectory()
    {
        string path = Directory.GetCurrentDirectory() + "\\" + settingsFolderName;

        if (Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
            Debug.Log($"No Save Directory found, Creating in {path}");
        }

        return path;
    }
    private string ValidateSettingsFile()
    {
        string path = ValidateSettingsDirectory() + "\\" + settingsFileName;

        if (File.Exists(path) == false)
        {
            var myFile = File.Create(path);
            myFile.Close();
            Debug.Log($"Save file created at path {path}");
        }

        return path;
    }
    public void OpenSettingsFolder()
    {
        ValidateSettingsDirectory();

        string path = Directory.GetCurrentDirectory() + "\\" + settingsFolderName;
        System.Diagnostics.Process.Start("explorer.exe", @path);
    }



    public TodayData GetTodayData(DateTime dateTime)
    {
        string path = ValidateDataFile(dateTime);

        TodayData data;

        data = JsonConvert.DeserializeObject<TodayData>(File.ReadAllText(path));

        if (data == null)
        {
            data = new TodayData(dateTime);
            SetTodayData(data);
        }

        return data;
    }
    public void SetTodayData(TodayData data)
    {
        File.WriteAllText(ValidateDataFile(data.timestamp), JsonConvert.SerializeObject(data, Formatting.Indented));
    }
    private string ValidateDataDirectory(DateTime dateTime)
    {
        string path = dataRootPath + "\\" + dataFolderName + "\\" + dateTime.Year + "\\" + dateTime.ToString("M - MMMM") + "\\" + dateTime.ToString("d - MMMM");

        if (Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
            Debug.Log($"No Save Directory found, Creating in {path}");
        }

        return path;
    }
    private string ValidateDataFile(DateTime dateTime)
    {
        string path = ValidateDataDirectory(dateTime) + "\\" + dateTime.ToString("d").Replace("/", "-") + ".Today";

        if (File.Exists(path) == false)
        {
            var myFile = File.Create(path);
            myFile.Close();
            Debug.Log($"Save file created at path {path}");
        }

        return path;
    }
    public List<TodayData> GetYearData()
    {
        List<TodayData> yearData = new List<TodayData>();


        DateTime startDate = new DateTime(DateTime.Now.Year, 1, 1);
        DateTime endDate = new DateTime(DateTime.Now.Year, 12, 31);

        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
        {
            string path = dataRootPath + "\\" + dataFolderName + "\\" + date.Year + "\\" + date.ToString("M - MMMM") + "\\" + date.ToString("d - MMMM") + "\\" + date.ToString("d").Replace("/", "-") + ".Today"; ;

            if (File.Exists(path) == false)
            {
                continue;
            }
            else
            {
                TodayData data = JsonConvert.DeserializeObject<TodayData>(File.ReadAllText(path));

                if (data == null)
                {
                    continue;
                }
                else
                {
                    yearData.Add(data);
                }
            }
        }

        String log = "TodayData Collected for\n";
        foreach (TodayData todayData in yearData)
        {
            log += $"{todayData.timestamp.ToString("d - MMMM")}\n";
        }

        Debug.Log(log);

        string yearDataPath = dataRootPath + "\\" + dataFolderName + "\\" + DateTime.Now.Year + "\\" + "YearData.Today";
        var myFile = File.Create(yearDataPath);
        myFile.Close();
        File.WriteAllText(yearDataPath, JsonConvert.SerializeObject(yearData, Formatting.Indented));

        return yearData;
    }


    public byte[] GetTodayImageData(DateTime dateTime)
    {
        if (ValidateImageFile(dateTime, out string path) == false) //no img found
        {
            return null;
        }

        return File.ReadAllBytes(path);
    }
    public byte[] SetNewTodayImageData(DateTime dateTime)
    {
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg" )};

        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);

        if (paths.Length == 0)
        {
            Debug.Log("No File Selected");
            return null;
        }
        else
        {
            string localPath = ValidateDataDirectory(dateTime) + "\\" + dateTime.ToString("d").Replace("/", "-") + ".png";

            FileStream localImageFile = File.Create(localPath);
            localImageFile.Close();

            File.WriteAllBytes(localPath, File.ReadAllBytes(paths[0]));

            return File.ReadAllBytes(paths[0]);
        }
    }
    private bool ValidateImageFile(DateTime dateTime, out string path)
    {
        path = ValidateDataDirectory(dateTime) + "\\" + dateTime.ToString("d").Replace("/", "-") + ".png";

        if (File.Exists(path) == false)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void OpenDataFolder()
    {
        string path = dataRootPath + "\\" + dataFolderName;
        System.Diagnostics.Process.Start("explorer.exe", @path);
    }
}
