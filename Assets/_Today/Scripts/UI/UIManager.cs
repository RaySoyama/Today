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
    private TMP_InputField todaysDiaryInputFeild = null;


    [Header("Todays Image")]

    [SerializeField]
    private RawImage todaysImage = null;

    [SerializeField]
    private AspectRatioFitter todaysImageAspectFitter = null;

    [SerializeField]
    private Color noImageColor;

    [SerializeField]
    private GameObject todayImageExpandedParent = null;

    [SerializeField]
    private RawImage todayImageExpanded = null;

    [SerializeField]
    private AspectRatioFitter todaysImageExpandedAspectFitter = null;


    [Header("Caffeine")]

    [SerializeField]
    private GameObject caffeineKeyParent = null;

    [SerializeField, ReadOnlyField]
    private List<CaffeineKey> allCaffeineKeys = new List<CaffeineKey>();

    [Header("GOTD")]

    [SerializeField]
    private TMP_Dropdown gameOfDayDropdown = null;

    [SerializeField]
    private TMP_InputField newGameTitleInputField = null;

    [Header("Other")]
    [SerializeField]
    private TextMeshProUGUI versionText = null;

    [SerializeField, Min(0.0f)]
    float doubleTapTime = 0.5f;
    float tapTimestamp = 0;

    //Prefab Ref
    [Header("Prefab Refs"), Space(10)]
    [SerializeField]
    private GameObject caffeineKeyPrefab = null;

    [Header("Cache")]

    [SerializeField, ReadOnlyField]
    private Texture2D todaysImageTexture = null;


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

        if (QDebug.IsNull(todaysDiaryInputFeild, nameof(todaysDiaryInputFeild), this) == false)
        {
            todaysDiaryInputFeild.onEndEdit.AddListener(TodayManager.Instance.OnDiaryEntry);
        }

        if (QDebug.IsNull(gameOfDayDropdown, nameof(gameOfDayDropdown), this) == false)
        {
            gameOfDayDropdown.onValueChanged.AddListener(OnGOTDUpdate);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.time - tapTimestamp < doubleTapTime) // double tap
            {
                OnQuit();
            }
            else
            {
                tapTimestamp = Time.time;
            }
        }
    }

    public void InitializeUI()
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


        if (QDebug.IsNull(todaysDiaryInputFeild, nameof(todaysDiaryInputFeild), this) == false)
        {
            todaysDiaryInputFeild.SetTextWithoutNotify(TodayManager.Instance.TodayData.diary);
        }

        if (QDebug.IsNull(gameOfDayDropdown, nameof(gameOfDayDropdown), this) == false)
        {
            gameOfDayDropdown.ClearOptions();
            gameOfDayDropdown.AddOptions(SettingsManager.Instance.SettingsData.gameOfDayTitles);

            if (SettingsManager.Instance.SettingsData.gameOfDayTitles.Contains(TodayManager.Instance.TodayData.gameOfTheDay) == false)
            {
                //wtf. add the game I guess?
                SettingsManager.Instance.AddNewGameOfDayTitle(TodayManager.Instance.TodayData.gameOfTheDay);

                if (QDebug.IsNull(gameOfDayDropdown, nameof(gameOfDayDropdown), this) == false)
                {
                    gameOfDayDropdown.ClearOptions();
                    gameOfDayDropdown.AddOptions(SettingsManager.Instance.SettingsData.gameOfDayTitles);
                }
            }

            gameOfDayDropdown.value = SettingsManager.Instance.SettingsData.gameOfDayTitles.IndexOf(TodayManager.Instance.TodayData.gameOfTheDay);
        }


        if (QDebug.IsNull(versionText, nameof(versionText), this) == false)
        {
            versionText.text = $"Build v{Application.version} Data v{TodayManager.Instance.TodayData.version}";
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
    public void SetTodaysImage(bool isDefault)
    {
        if (isDefault == true)
        {
            todaysImage.texture = null;
            todaysImage.color = noImageColor;
            todaysImageAspectFitter.aspectRatio = 16.0f / 9.0f;

            todayImageExpanded.texture = null;
            todaysImageExpandedAspectFitter.aspectRatio = 16.0f / 9.0f;
        }
        else
        {
            todaysImageTexture = new Texture2D(1, 1);
            todaysImageTexture.LoadImage(TodayManager.Instance.TodaysImage);

            todaysImage.texture = todaysImageTexture;
            todaysImage.color = Color.white;
            todaysImageAspectFitter.aspectRatio = (float)todaysImageTexture.width / (float)todaysImageTexture.height;

            todayImageExpanded.texture = todaysImageTexture;
            todaysImageExpandedAspectFitter.aspectRatio = (float)todaysImageTexture.width / (float)todaysImageTexture.height;

        }
    }
    public void ToggleTodayImageExpand(bool expand)
    {
        if (todaysImage.texture == null)
        {
            //this can soft locK?
            return;
        }

        todayImageExpandedParent.SetActive(expand);
    }

    public void AddNewGameTile()
    {
        if (newGameTitleInputField.text == "" || SettingsManager.Instance.SettingsData.gameOfDayTitles.Contains(newGameTitleInputField.text))
        {
            return;
        }
        //add to list
        SettingsManager.Instance.AddNewGameOfDayTitle(newGameTitleInputField.text);

        if (QDebug.IsNull(gameOfDayDropdown, nameof(gameOfDayDropdown), this) == false)
        {
            gameOfDayDropdown.ClearOptions();
            gameOfDayDropdown.AddOptions(SettingsManager.Instance.SettingsData.gameOfDayTitles);

            gameOfDayDropdown.value = SettingsManager.Instance.SettingsData.gameOfDayTitles.IndexOf(newGameTitleInputField.text);
        }

        newGameTitleInputField.text = "";
    }
    public void OnGOTDUpdate(int value)
    {
        //cringe but just relay event call
        TodayManager.Instance.OnGOTDUpdate(gameOfDayDropdown.options[value].text);
    }


    public void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
