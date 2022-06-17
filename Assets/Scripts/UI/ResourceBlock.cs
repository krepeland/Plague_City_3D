using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBlock : MonoBehaviour
{
    public static Dictionary<EResource, ResourceBlock> ResourceBlocks = new Dictionary<EResource, ResourceBlock>();

    public EResource ResourceType;
    public Image ResourceIcon;
    public Image ResourceIconOutline;
    public Text CountText;
    public Text CountTextBackground;

    public Image YellowAlert;
    public Image RedAlert;

    public int ValueNow;
    public int DeltaNow;

    public int YellowAlertCount = 0;
    public int RedAlertCount = 0;

    private void Awake()
    {
        ResourceBlocks[ResourceType] = this;
    }

    public void UpdateValues() {
        ValueNow = ResourceManager.singleton.GetResourceCount(ResourceType);
        DeltaNow = ResourceManager.singleton.GetResourceDelta(ResourceType);


        if (ValueNow <= RedAlertCount)
        {
            RedAlert.enabled = true;
            YellowAlert.enabled = false;
        }
        else
        {
            if (ValueNow <= YellowAlertCount)
            {
                RedAlert.enabled = false;
                YellowAlert.enabled = true;
            }
            else
            {
                RedAlert.enabled = false;
                YellowAlert.enabled = false;
            }
        }

        UpdateTexts();
    }

    public void UpdateTexts() {
        var text = ValueNow.ToString();
        CountText.text = text;
        var zerosInFront = 6 - text.Length;
        for (var i = 0; i < zerosInFront; i++) {
            text = '0' + text;
        }
        CountTextBackground.text = text;
    }
}
