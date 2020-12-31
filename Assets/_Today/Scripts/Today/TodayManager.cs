using System;
using UnityEngine;

public class TodayManager : MonoBehaviour
{
    private static TodayManager instnace = null;
    public static TodayManager Instance
    {
        get
        {
            QDebug.IsNull(instnace, "TodayManager Instance", null, QDebug.Severity.OhGodWhy);

            return instnace;
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


    void Awake()
    {
        if (instnace == null)
        {
            instnace = this;
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
}
