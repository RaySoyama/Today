using TMPro;
using UnityEngine;
public class CaffeineKey : MonoBehaviour
{
    [SerializeField, ReadOnlyField]
    private TodayData.CaffeineType caffeineKeyType = TodayData.CaffeineType.Coffee;
    public TodayData.CaffeineType CaffeineKeyType
    {
        get
        {
            return caffeineKeyType;
        }
    }




    [SerializeField, ReadOnlyField]
    private int index = 0;
    public int Index
    {
        get
        {
            return index;
        }
    }


    //I hate this
    [Space(10)]
    [SerializeField]
    private UnityEngine.UI.Button button = null;
    [SerializeField]
    private UnityEngine.UI.Image image = null;


    [SerializeField]
    private TextMeshProUGUI typeText = null;

    public void Initialize(TodayData.CaffeineType _type, int _index)
    {
        caffeineKeyType = _type;
        index = _index;

        //Update Text
        string text = "";

        switch (caffeineKeyType)
        {
            case TodayData.CaffeineType.Coffee:
                text = "C";
                break;
            case TodayData.CaffeineType.Tea:
                text = "T";
                break;
            case TodayData.CaffeineType.EnergyDrink:
                text = "E";
                break;
            case TodayData.CaffeineType.Other:
                text = "O";
                break;
        }

        if (QDebug.IsNull(typeText, nameof(typeText), this) == false)
        {
            typeText.text = text;
        }


        if (QDebug.IsNull(button, nameof(button), this) == false)
        {
            button.enabled = true;
        }

        if (QDebug.IsNull(image, nameof(image), this) == false)
        {
            image.enabled = true;
        }
    }

    //Delete
    public void OnDelete()
    {
        UIManager.Instance.DeleteCaffeineData(this);

        Destroy(this.gameObject);
    }
}
