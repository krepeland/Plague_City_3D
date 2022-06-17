using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BuyBuildingInfo : MonoBehaviour
{
    public Image BackgroundImage;
    public Image BackgroundOutLine;
    public Image ArrowImage;
    public Text DescriptionText;
    public Text ConditionTextText;
    public Text ConditionText;

    private bool isVisible;
    float visibleValue = 0;
    float previousValue = -1;

    private Transform targetObject;
    private RectTransform rect;
    [SerializeField] private BuildingsShop buildingsShop;

    public static BuyBuildingInfo singleton;

    static int LineStep = 20;

    private void Awake()
    {
        singleton = this;
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        visibleValue = Mathf.Clamp01(visibleValue + (isVisible ? Time.deltaTime : -Time.deltaTime) * 5);

        if (previousValue != visibleValue) {
            previousValue = visibleValue;
            BackgroundImage.color = new Color(0, 0, 0, visibleValue * 0.4f);
            BackgroundOutLine.color = new Color(BackgroundOutLine.color.r, BackgroundOutLine.color.g, BackgroundOutLine.color.b, visibleValue * 0.8f);
            ArrowImage.color = new Color(0, 0, 0, visibleValue * 0.4f);

            DescriptionText.color = new Color(1, 1, 1, visibleValue);
            ConditionText.color = new Color(1, 1, 1, visibleValue);

            ConditionTextText.color = new Color(0.7f, 0.7f, 0.7f, visibleValue);
        }
        if (targetObject != null)
        {
            transform.position = targetObject.transform.position;

            var x = Mathf.Clamp(rect.anchoredPosition.x, 160, buildingsShop.BuildingShopTarget2.anchoredPosition.x - 160);
            rect.anchoredPosition = new Vector2(x, 40);

            ArrowImage.transform.position = targetObject.transform.position;
            ArrowImage.rectTransform.anchoredPosition = new Vector2(ArrowImage.rectTransform.anchoredPosition.x, 0);
        }
    }

    public void SetInfo(BuyCardData buyCardData, CardData cardData, Transform targetObject, bool isResourcesRequired) {
        this.targetObject = targetObject;
        transform.position = targetObject.transform.position;

        var x = Mathf.Clamp(rect.anchoredPosition.x, 160, buildingsShop.BuildingShopTarget2.anchoredPosition.x - 160);
        rect.anchoredPosition = new Vector2(x, 40);

        ArrowImage.transform.position = targetObject.transform.position;
        ArrowImage.rectTransform.anchoredPosition = new Vector2(ArrowImage.rectTransform.anchoredPosition.x, 0);

        DescriptionText.text = cardData.Description;
        if (buyCardData != null && (isResourcesRequired || buyCardData.BuildingsRequired.Count != 0))
        {
            ConditionText.text = GetCardConditionText(buyCardData, isResourcesRequired);
            ConditionTextText.enabled = true;
        }
        else {
            ConditionText.text = "";
            ConditionTextText.enabled = false;
        }

        Canvas.ForceUpdateCanvases();
        var descriptionLines = DescriptionText.cachedTextGenerator.lines.Count;
        var conditionLines = ConditionText.cachedTextGenerator.lines.Count;

        if (buyCardData == null || (!isResourcesRequired && buyCardData.BuildingsRequired.Count == 0)) {
            conditionLines -= 3;
        }
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (descriptionLines + conditionLines + 4) * LineStep);
        DescriptionText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, descriptionLines * LineStep);
    }

    string GetCardConditionText(BuyCardData buyCardData, bool isResourcesRequired) {
        var builder = new System.Text.StringBuilder();
        if (isResourcesRequired)
        {
            for (var i = 0; i < buyCardData.ResourcesRequired.Count; i++)
            {
                var e = buyCardData.ResourcesRequired[i];
                if (i != 0)
                    builder.Append(", ");
                builder.Append(e.Count);
                builder.Append(" ");
                builder.Append(ResourceManager.GetResourceName(e.Resource));
            }
        }

        for (var i = 0; i < buyCardData.BuildingsRequired.Count; i++)
        {
            var e = buyCardData.BuildingsRequired[i];
            if(isResourcesRequired || i != 0)
                builder.Append("\n");
            switch (e.CountComparison) {
                case CountComparison.EqualTo:
                    builder.Append(
                        $"Placed {e.Count} \"{e.CardName}\" (now {CardsManager.singleton.GetSettedBuildingsCount(e.CardName)})");
                    break;
                case CountComparison.MoreOrEqualTo:
                    builder.Append(
                        $"Placed at least {e.Count} \"{e.CardName}\" (now {CardsManager.singleton.GetSettedBuildingsCount(e.CardName)}/{e.Count})");
                    break;
                case CountComparison.NoMoreThan:
                    builder.Append(
                        $"Placed no more than {e.Count} \"{e.CardName}\" (now {CardsManager.singleton.GetSettedBuildingsCount(e.CardName)}/{e.Count})");
                    break;
            }
        }

        return builder.ToString();
    }

    public void SetVisible(bool isVisible) {
        this.isVisible = isVisible;
    }
}
