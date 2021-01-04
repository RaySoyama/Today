using System;
using UnityEngine;

public class TodayManager : MonoBehaviour
{
    private static TodayManager instance = null;
    public static TodayManager Instance
    {
        get
        {
            QDebug.IsNull(instance, "TodayManager Instance", null, QDebug.Severity.OhGodWhy);

            return instance;
        }
    }


    [SerializeField]
    private string version = "00.00.00";
    public string Version
    {
        get
        {
            return version;
        }
    }

    [SerializeField]
    private string targetDateString = "";
    private DateTime targetDate;

    [Space(20), SerializeField]
    private TodayData todayData = null;
    public TodayData TodayData
    {
        get
        {
            return todayData;
        }
    }

    private byte[] todaysImage;
    public byte[] TodaysImage
    {
        get
        {
            return todaysImage;
        }
    }



    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Multiple Instances of TodayManager Singleton");
            Destroy(this);
        }
    }
    void Start()
    {
        targetDate = DateTime.Now;
        targetDateString = targetDate.ToString("d");

        InitializeTodayData();
    }

    private void InitializeTodayData()
    {
        todayData = DataIOManager.Instance.GetTodayData(targetDate);

        if (todayData == null)
        {
            todayData = new TodayData(DateTime.Now);
            DataIOManager.Instance.SetTodayData(todayData);
        }

        todaysImage = DataIOManager.Instance.GetTodayImageData(targetDate);

        if (todaysImage == null)
        {
            UIManager.instance.SetTodaysImage(true);
        }
        else
        {
            UIManager.instance.SetTodaysImage(false);
        }

        UIManager.instance.InitializeUI();
    }

    public void GetTodayData()
    {
        targetDate = DateTime.Parse(targetDateString);
        InitializeTodayData();
    }

    public void SaveTodayData()
    {
        DataIOManager.Instance.SetTodayData(todayData);
    }

    public void AddCaffeineData(TodayData.CaffeineType type)
    {
        todayData.caffeineData.Add(type);
        DataIOManager.Instance.SetTodayData(todayData);
    }
    public void DeleteCaffeineData(TodayData.CaffeineType type, int index)
    {
        //check if index in bounds
        if (index >= 0 && index < todayData.caffeineData.Count && todayData.caffeineData[index] == type)
        {
            todayData.caffeineData.RemoveAt(index);
            DataIOManager.Instance.SetTodayData(todayData);
        }
        else
        {
            //error
            Debug.LogError("Trying to remove invalid Caffeine Data");
        }
    }

    public void SetMotivationValue(float val)
    {
        todayData.motivation = (int)val;
        DataIOManager.Instance.SetTodayData(todayData);
    }
    public void SetHappinessValue(float val)
    {
        todayData.happiness = (int)val;
        DataIOManager.Instance.SetTodayData(todayData);
    }


    //Button Area
    public void OnUserSelectTodaysImage()
    {
        todaysImage = DataIOManager.Instance.SetNewTodayImageData(targetDate);

        if (todaysImage == null)
        {
            //really garbage but eh.
            //if no image selected, see if theres already a default image saved

            todaysImage = DataIOManager.Instance.GetTodayImageData(targetDate);

            if (todaysImage == null)
            {
                UIManager.instance.SetTodaysImage(true);
            }
            else
            {
                UIManager.instance.SetTodaysImage(false);
            }
        }
        else
        {
            UIManager.instance.SetTodaysImage(false);
        }
    }
    public void OnLoadYesterday()
    {
        targetDate = targetDate.AddDays(-1);
        InitializeTodayData();
    }
    public void OnLoadTomorrow()
    {
        if (DateTime.Now.Date < targetDate.AddDays(1).Date)
        {
            //Can't load into the future
            return;
        }

        targetDate = targetDate.AddDays(1);
        InitializeTodayData();
    }

    public void OnDiaryEntry(string text)
    {
        todayData.diary = text;
        DataIOManager.Instance.SetTodayData(todayData);
    }

    public void OnGOTDUpdate(string title)
    {
        todayData.gameOfTheDay = title;
        DataIOManager.Instance.SetTodayData(todayData);
    }

}
