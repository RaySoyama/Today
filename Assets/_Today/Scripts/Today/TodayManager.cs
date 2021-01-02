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


        todayData = DataIOManager.Instance.GetTodayData(targetDate);

        if (todayData == null)
        {
            todayData = new TodayData(DateTime.Now);
            DataIOManager.Instance.SetTodayData(todayData);
        }
    }

    public void GetTodayData()
    {
        targetDate = DateTime.Parse(targetDateString);
        todayData = DataIOManager.Instance.GetTodayData(targetDate);
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
}
