using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BI_TimedAction : IBuildingInfoBlock
{
    public Text text;
    public Image TimerFiller;
    public Text TimeNowText;

    float part = 0;

    public void Init(string text, int timeMax, int timeNow)
    {
        this.text.text = text;
        part = 1f / timeMax;
        UpdateTimeNow(timeNow);
    }

    public void UpdateTimeNow(int timeNow) {
        TimerFiller.fillAmount = part * timeNow;
        TimeNowText.text = timeNow.ToString();
    }

    public override int GetHeightSize()
    {
        return 85;
    }

    public override BuildingInfoBlockType GetBuildingInfoBlockType()
    {
        return BuildingInfoBlockType.TimedAction;
    }
}
