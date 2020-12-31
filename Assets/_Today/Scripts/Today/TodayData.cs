using System;
using System.Collections.Generic;

[Serializable]
public class TodayData
{
    public enum CaffeineType
    {
        Coffee = 1,
        Tea = 2,
        EnergyDrink = 3,
        Other = 4
    }

    [ReadOnlyField]
    public string version = "00.00.00";

    public DateTime timestamp = new DateTime();

    public List<CaffeineType> caffeineData = new List<CaffeineType>();

    [UnityEngine.Range(0, 7)]
    public int motivation = 4;

    [UnityEngine.Range(0, 7)]
    public int happiness = 4;

    public string gameOfTheDay = "N/A";

    [UnityEngine.TextArea(10, 20)]
    public string diary = "";

    public TodayData(DateTime dateTime)
    {
        timestamp = dateTime;
        version = TodayManager.Instance.Version;
    }

}
