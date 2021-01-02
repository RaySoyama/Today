using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;
    public static UIManager Instance
    {
        get
        {
            QDebug.IsNull(instance, "UIManager Instance", null, QDebug.Severity.OhGodWhy);

            return instance;
        }
    }

    [SerializeField]
    private TextMeshProUGUI activeDateText = null;

    [SerializeField]
    private TextMeshProUGUI actualDateText = null;

    [SerializeField]
    private Slider motivationSlider = null;

    [SerializeField]
    private Slider happinessSlider = null;











    [SerializeField]
    private GameObject caffeineKeyParent = null;

    [SerializeField, ReadOnlyField]
    private List<CaffeineKey> allCaffeineKeys = new List<CaffeineKey>();


    //Prefab Ref
    [Header("Prefab Refs"), Space(10)]
    [SerializeField]
    private GameObject caffeineKeyPrefab = null;



    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Multiple Instances of UIManager Singleton");
            Destroy(this);
        }
    }
    void Start()
    {
        if (QDebug.IsNull(motivationSlider, nameof(motivationSlider), this) == false)
        {
            motivationSlider.onValueChanged.AddListener(TodayManager.Instance.SetMotivationValue);
        }

        if (QDebug.IsNull(happinessSlider, nameof(happinessSlider), this) == false)
        {
            happinessSlider.onValueChanged.AddListener(TodayManager.Instance.SetHappinessValue);
        }

        InitializeUI();
    }

    void InitializeUI()
    {
        if (QDebug.IsNull(activeDateText, nameof(activeDateText), this) == false)
        {
            activeDateText.text = $"{TodayManager.Instance.TodayData.timestamp.ToString("dddd, MMMM dd")}";
        }

        if (QDebug.IsNull(actualDateText, nameof(actualDateText), this) == false)
        {
            actualDateText.text = $"Today is\n({DateTime.Now.ToString("dddd, MMMM dd")})";

            if (TodayManager.Instance.TodayData.timestamp.Date != DateTime.Now.Date)
            {
                actualDateText.gameObject.SetActive(true);
            }
            else
            {
                actualDateText.gameObject.SetActive(false);
            }
        }

        if (QDebug.IsNull(caffeineKeyParent, nameof(caffeineKeyParent), this) == false)
        {
            //Destroy Current Caffeine Keys
            allCaffeineKeys.Clear();

            foreach (Transform key in caffeineKeyParent.transform)
            {
                Destroy(key.gameObject);
            }

            //Spawn Caffeine Keys
            if (QDebug.IsNull(caffeineKeyPrefab, nameof(caffeineKeyPrefab), this) == false)
            {
                //spawn caffeine key 
                for (int i = 0; i < TodayManager.Instance.TodayData.caffeineData.Count; i++)
                {
                    CaffeineKey newKey = Instantiate(caffeineKeyPrefab, caffeineKeyParent.transform).GetComponent<CaffeineKey>();
                    allCaffeineKeys.Add(newKey);
                    newKey.Initialize(TodayManager.Instance.TodayData.caffeineData[i], i);
                }
            }
        }

        if (QDebug.IsNull(motivationSlider, nameof(motivationSlider), this) == false)
        {
            motivationSlider.value = TodayManager.Instance.TodayData.motivation;
        }

        if (QDebug.IsNull(happinessSlider, nameof(happinessSlider), this) == false)
        {
            happinessSlider.value = TodayManager.Instance.TodayData.happiness;
        }
    }
    public void AddCaffeineData(int type)
    {
        if (QDebug.IsNull(caffeineKeyParent, nameof(caffeineKeyParent), this) == false && QDebug.IsNull(caffeineKeyPrefab, nameof(caffeineKeyPrefab), this) == false)
        {
            //instantiate new caffeine key button
            CaffeineKey newKey = Instantiate(caffeineKeyPrefab, caffeineKeyParent.transform).GetComponent<CaffeineKey>();
            allCaffeineKeys.Add(newKey);
            //this is super "hard coded" with he way the index is initialized
            newKey.Initialize((TodayData.CaffeineType)type, allCaffeineKeys.Count - 1);

            TodayManager.Instance.AddCaffeineData((TodayData.CaffeineType)type);
        }
    }
    public void DeleteCaffeineData(CaffeineKey key)
    {
        allCaffeineKeys.Remove(key);

        //I have a feeling this might break
        for (int i = 0; i < allCaffeineKeys.Count; i++)
        {
            allCaffeineKeys[i].Initialize(allCaffeineKeys[i].CaffeineKeyType, i);
        }

        TodayManager.Instance.DeleteCaffeineData(key.CaffeineKeyType, key.Index);
    }


}
