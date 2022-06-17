using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingsShopBlock : CustomButton
{
    public bool IsUnlocked;

    public Text CardName;
    public Image CardImage;
    public Image CardBackground;

    public Color CardNameColorUnlocked;
    public Color CardNameColorLocked;

    public Color CardBackgroundColorUnlocked;
    public Color CardBackgroundColorLocked;

    public BuyCardData BuyCardData;
    public CardData CardData;

    private void Start()
    {
        SetIsUnlocked(IsUnlocked);
    }

    public void BuyCard(int takedWay)
    {
        if (CardsManager.singleton.SelectedCard != null)
            return;

        CheckIsUnlocked();
        if (!IsUnlocked)
            return;

        if (!CardsManager.singleton.IsLessThanMaxCardsCount())
            return;

        foreach (var e in BuyCardData.ResourcesRequired) {
            ResourceManager.singleton.AddResourceCount(e.Resource, -e.Count, true);    
        }

        var newCard = CardsManager.singleton.SpawnGameCard(CardData);
        CardsManager.singleton.TakedCard(newCard, takedWay, false);
        newCard.SetTargetPos(new Vector3(130, -120, 0), 1000);
    }

    public void SetBuildingShopData(BuyCardData buyCardData) {
        BuyCardData = buyCardData;
        CardData = CardsManager.singleton.GetCardData(buyCardData);
        CardName.text = CardData.CardName;

        if (IsUnlocked)
        {
            if (buyCardData.UnlockedSprite != null)
            {
                CardImage.color = Color.white;
                CardImage.sprite = buyCardData.UnlockedSprite;
            }
            else {
                CardImage.color = new Color(0.31f, 0.31f, 0.31f, 1);
            }
        }
        else
        {
            if (buyCardData.LockedSprite != null)
            {
                CardImage.color = Color.white;
                CardImage.sprite = buyCardData.LockedSprite;
            }
            else
            {
                CardImage.color = new Color(0.31f, 0.31f, 0.31f, 1);
            }
        }

        CheckIsUnlocked();
    }

    public void CheckIsUnlocked() {
        var result = true;
        foreach (var e in BuyCardData.ResourcesRequired) {
            if (!ResourceManager.singleton.IsHaveEnoughResources(e.Resource, e.Count, out _))
            {
                result = false;
                break;
            }
        }

        foreach (var e in BuyCardData.BuildingsRequired)
        {
            switch (e.CountComparison)
            {
                case CountComparison.EqualTo:
                    if (CardsManager.singleton.GetSettedBuildingsCount(e.CardName) != e.Count)
                        result = false;
                    break;
                case CountComparison.MoreOrEqualTo:
                    if (CardsManager.singleton.GetSettedBuildingsCount(e.CardName) < e.Count)
                        result = false;
                    break;
                case CountComparison.NoMoreThan:
                    if (CardsManager.singleton.GetSettedBuildingsCount(e.CardName) > e.Count)
                        result = false;
                    break;
            }
        }

        SetIsUnlocked(result);
    }

    public void SetIsUnlocked(bool isUnlocked) {
        IsUnlocked = isUnlocked;

        CardName.color = isUnlocked ? CardNameColorUnlocked : CardNameColorLocked;
        CardBackground.color = isUnlocked ? CardBackgroundColorUnlocked : CardBackgroundColorLocked;
    }

    public override void OnHover()
    {
        transform.localScale = Vector3.one * 1.1f;
        BuyBuildingInfo.singleton.SetVisible(true);
        BuyBuildingInfo.singleton.SetInfo(BuyCardData, CardData, transform, true);
    }

    public override void OutHover()
    {
        transform.localScale = Vector3.one;
        BuyBuildingInfo.singleton.SetVisible(false);
    }

    public override void OnClicked(int mouseButton)
    {
        if (IsUnlocked && mouseButton == 0)
        {
            BuyCard(0);
        }
        else { 
        
        }
    }
}
