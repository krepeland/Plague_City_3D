using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitActionInfo : MonoBehaviour
{
    public static UnitActionInfo singleton;

    Dictionary<UnitActionType, UnitActionInfoData> unitActionInfoData; 

    public Image[] Image;
    public Image[] OutlineImage;
    public Text[] TimeText;

    public Text PlusText;

    private void Awake()
    {
        singleton = this;
        unitActionInfoData = new Dictionary<UnitActionType, UnitActionInfoData>();
        foreach (var data in Resources.LoadAll<UnitActionInfoData>("UnitActionsData")) {
            unitActionInfoData[data.ActionType] = data;
        }

        SetAction(0, UnitActionType.None);
        SetAction(1, UnitActionType.None);
    }

    public void SetAction(int index, UnitActionType unitActionType) {
        bool isActive = unitActionType != UnitActionType.None;
        if (index == 1) {
            PlusText.gameObject.SetActive(isActive);
        }

        Image[index].gameObject.SetActive(isActive);
        OutlineImage[index].gameObject.SetActive(isActive);
        TimeText[index].gameObject.SetActive(isActive);

        if (isActive)
        {
            Image[index].sprite = unitActionInfoData[unitActionType].Sprite;
            OutlineImage[index].sprite = unitActionInfoData[unitActionType].SpriteOutline;
        }
    }

    public void SetActionText(int index, string time) {
        TimeText[index].text = time;
    }
}
