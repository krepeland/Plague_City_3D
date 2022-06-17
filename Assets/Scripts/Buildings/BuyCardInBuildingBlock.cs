using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyCardInBuildingBlock : CustomButton
{
    public Text CardNameText;
    public Text CostText;
    public Image CardImage;
    public Image CardBackground;
    public Image Filler;

    public Color CardBackgroundColorUnlocked;
    public Color CardBackgroundColorLocked;

    public Transform Card;

    bool isLocked;

    public BuyCardData BuyCardData;
    BuyCardInBuilding owner;

    public void Init(BuyCardData buyCardData, BuyCardInBuilding owner) {
        this.owner = owner;

        BuyCardData = buyCardData;

        CardNameText.text = BuyCardData.CardName;
        //CardImage.sprite = isLocked ? BuyCardData.LockedSprite : BuyCardData.UnlockedSprite;

        CostText.text = GetCostString();
    }

    string GetCostString() {
        var line = "";
        foreach (var resourceRequired in BuyCardData.ResourcesRequired) {
            line += $"{resourceRequired.Count} {ResourceManager.GetResourceName(resourceRequired.Resource)}\n";
        }
        return line;
    }

    public void UpdateCheckIsLocked() {
        var result = false;
        if (owner.ÑardsCountLeftNow <= 0)
        {
            result = true;
        }
        else
        {
            foreach (var e in BuyCardData.ResourcesRequired)
            {
                if (!ResourceManager.singleton.IsHaveEnoughResources(e.Resource, e.Count, out _))
                {
                    result = true;
                    break;
                }
            }
        }
        UpdateIsLocked(result);
    }

    void UpdateIsLocked(bool isLocked)
    {
        this.isLocked = isLocked;
        Filler.gameObject.SetActive(isLocked);
        if (isLocked) {
            Card.localScale = Vector3.one;
        }


        if (!isLocked)
        {
            if (BuyCardData.UnlockedSprite != null)
            {
                CardImage.color = Color.white;
                CardImage.sprite = BuyCardData.UnlockedSprite;
            }
            else
            {
                CardImage.color = new Color(0.31f, 0.31f, 0.31f, 1);
            }
        }
        else
        {
            if (BuyCardData.LockedSprite != null)
            {
                CardImage.color = Color.white;
                CardImage.sprite = BuyCardData.LockedSprite;
            }
            else
            {
                CardImage.color = new Color(0.31f, 0.31f, 0.31f, 1);
            }
        }
        CardBackground.color = isLocked ? CardBackgroundColorLocked : CardBackgroundColorUnlocked;
    }

    public void BuyCard(int takedWay)
    {
        if (CardsManager.singleton.SelectedCard != null)
            return;

        if (isLocked)
            return;

        if (!CardsManager.singleton.IsLessThanMaxCardsCount())
            return;

        foreach (var e in BuyCardData.ResourcesRequired)
        {
            ResourceManager.singleton.AddResourceCount(e.Resource, -e.Count, true);
        }
        owner.ÑardsCountLeftNow -= 1;

        var newCard = CardsManager.singleton.SpawnGameCard(CardsManager.singleton.GetCardData(BuyCardData));
        CardsManager.singleton.TakedCard(newCard, takedWay, false);
        newCard.SetTargetPos(new Vector3(130, -120, 0), 1000);
        BuildingInfo.singleton.CheckAllIsUnlocked();
    }

    public override void OnClicked(int mouseButton)
    {
        if (mouseButton == 0)
        {
            BuyCard(mouseButton);
        }
    }


    public override void OnHover()
    {
        if (isLocked || CardsManager.singleton.SelectedCard != null)
            return;
        Card.localScale = Vector3.one * 1.05f;
    }

    public override void OutHover()
    {
        Card.localScale = Vector3.one;
    }
}
