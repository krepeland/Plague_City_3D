using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BI_BuyCards : IBuildingInfoBlock
{
    public Text text;

    public Image TimerFiller;
    public Text TimeNowText;
    public RectTransform background;

    private int size = 85;
    private static int sizePerLine = 128;

    public Transform Container;

    float part = 0;
    List<BuyCardData> buyCardDatas;

    Transform prefabBuyCardInBuilding;
    List<BuyCardInBuildingBlock> blocks;

    public void Init(string text, int timeMax, int timeNow, List<BuyCardData> buyCardDatas, BuyCardInBuilding buyCardInBuilding)
    {
        blocks = new List<BuyCardInBuildingBlock>();

        prefabBuyCardInBuilding = PrefabManager.GetPrefab("BuildingInfoBlocks_BuyCardInBuildingContainer");
        part = 1f / timeMax;
        UpdateTimeNow(text, timeNow);
        this.buyCardDatas = buyCardDatas;

        var x = 0;
        var y = 0;

        foreach (var e in this.buyCardDatas) {
            var nextCard = Instantiate(prefabBuyCardInBuilding, Container);
            nextCard.GetComponent<RectTransform>().anchoredPosition = new Vector2((x == 0 ? -92 : 92), -sizePerLine * y);

            if (x == 1)
            {
                y++;
            }
            x = (x + 1) % 2;

            var block = nextCard.GetComponent<BuyCardInBuildingBlock>();
            nextCard.GetComponent<BuyCardInBuildingBlock>().Init(e, buyCardInBuilding);
            blocks.Add(block);
            block.UpdateCheckIsLocked();
        }

        if(this.buyCardDatas.Count % 2 == 0)
            size += sizePerLine * (this.buyCardDatas.Count / 2);
        else
            size += sizePerLine * ((this.buyCardDatas.Count + 1) / 2);
    }

    public void UpdateIsLocked()
    {
        foreach (var block in blocks)
        {
            block.UpdateCheckIsLocked();
        }
    }

    public void UpdateTimeNow(string text, int timeNow)
    {
        this.text.text = text;
        TimerFiller.fillAmount = part * timeNow;
        TimeNowText.text = timeNow.ToString();
    }

    public override int GetHeightSize()
    {
        return size;
    }

    public override void LoadBlockBeforeCanvasUpdate()
    {

    }

    public override void LoadBlockAfterCanvasUpdate()
    {
        //Size = 75;
        //var lineCount = text.cachedTextGenerator.lines.Count;
        //if (lineCount > 3) {
        //    Size += SizePerLine * (lineCount - 3);
        //}
        //background.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Size - SizePerLine);
    }

    public override BuildingInfoBlockType GetBuildingInfoBlockType()
    {
        return BuildingInfoBlockType.BuyCards;
    }
}
