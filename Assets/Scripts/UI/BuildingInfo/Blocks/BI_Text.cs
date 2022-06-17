using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BI_Text : IBuildingInfoBlock
{
    public Text text;
    public RectTransform background;
    public int Size = 40;
    public static int SizePerLine = 15;

    public void Init(string text)
    {
        this.text.text = text;
    }

    public override int GetHeightSize()
    {
        return Size;
    }

    public override void LoadBlockBeforeCanvasUpdate()
    {

    }

    public override void LoadBlockAfterCanvasUpdate()
    {
        Size = 40;
        Size += text.cachedTextGenerator.lines.Count * SizePerLine;
        background.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Size - SizePerLine);
    }

    public override BuildingInfoBlockType GetBuildingInfoBlockType()
    {
        return BuildingInfoBlockType.Text;
    }
}
