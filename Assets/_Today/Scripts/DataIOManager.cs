using Newtonsoft.Json;
using System;
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


        //load other files

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
        string path = dataRootPath + "\\" + dataFolderName + "\\" + dateTime.Year + "\\" + dateTime.ToString("MM - MMMM");

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
    public void OpenDataFolder()
    {
        string path = dataRootPath + "\\" + dataFolderName;
        System.Diagnostics.Process.Start("explorer.exe", @path);
    }
}
