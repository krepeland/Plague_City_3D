using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellCardBlock : CustomButton
{
    private bool IsMovingToTargetPos;
    private Vector2 AnchoredTargetPos = Vector2.zero;
    [HideInInspector] public float MoveToTargetSpeed = 100;
    private RectTransform rectTransform;
    private bool isSelling;
    public float TimeToSell = 1f;
    public float SellingTime = 0f;
    public Transform SellPosition;

    public Image SellingFiller;
    public Transform SellingFillerPos;
    public Text SellingText;
    public bool IsCanBeSold;

    public List<ResourceRequired> ResourcesFromSold;

    public static SellCardBlock singleton;

    private void Awake()
    {
        singleton = this;
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (IsMovingToTargetPos)
        {
            var dist = Vector2.Distance(rectTransform.anchoredPosition, AnchoredTargetPos);
            if (dist <= 0.1f || Time.deltaTime * MoveToTargetSpeed / dist >= 1)
            {
                IsMovingToTargetPos = false;
                rectTransform.anchoredPosition = AnchoredTargetPos;
            }
            else
            {
                rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, AnchoredTargetPos, Time.deltaTime * MoveToTargetSpeed / dist);
            }
            SellingFiller.transform.position = SellingFillerPos.position;
        }

        if (isSelling && IsCanBeSold)
        {
            var selectedCard = CardsManager.singleton.SelectedCard;
            if (selectedCard == null)
                return;

            selectedCard.transform.position = SellPosition.transform.position;
            SellingTime += Time.deltaTime;
            if (SellingTime >= TimeToSell)
                CardSold();
            SellingFiller.fillAmount = SellingTime / TimeToSell;
        }
        else {
            SellingFiller.fillAmount = 0;
        }
    }

    public bool LoadInfo(GameCard card) {
        if (CardsManager.singleton.HaveBuyCardData(card.CardData.CardName)) {
            var buyData = CardsManager.singleton.GetBuyCardData(card.CardData.CardName);
            if (!buyData.IsCanBeSold)
                return false;
            
            ResourcesFromSold = buyData.ResourcesFromSold;
            var text = "";
            foreach (var e in ResourcesFromSold) {
                if (e.Count > 0) {
                    text += $"+{e.Count} {ResourceManager.GetResourceName(e.Resource)}\n";
                }
                else
                {
                    text += $"{e.Count} {ResourceManager.GetResourceName(e.Resource)}\n";
                }
            }
            SellingText.text = text;
            Canvas.ForceUpdateCanvases();
            var lines = SellingText.cachedTextGenerator.lines.Count;
            if (lines > 0)
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 170 + (lines * 20));
            }
            else {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 205);
            }
            return true;
        }
        return false;
    }

    public void CardSold() {
        isSelling = false;
        SellingTime = 0;
        CardsManager.singleton.CardSold(ResourcesFromSold);
        ClearSellingBlock();
    }

    public override void OnHover()
    {
        base.OnHover();
        SetTargetMove(new Vector2(-4, 160), 500);

        var selectedCard = CardsManager.singleton.SelectedCard;
        if (selectedCard != null)
        {
            selectedCard.transform.localPosition = new Vector3(0, -80, 0);
            IsCanBeSold = LoadInfo(selectedCard);
            if (!IsCanBeSold)
            {
                ClearSellingBlock();
            }
        }
    }

    void ClearSellingBlock() {
        SellingText.text = "";
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 205);
    }

    public override void OutHover()
    {
        base.OutHover();
        SetTargetMove(new Vector2(140, 160), 500);

        var selectedCard = CardsManager.singleton.SelectedCard;
        if (selectedCard != null)
            selectedCard.transform.localPosition = new Vector3(130, -120, 0);

        isSelling = false;
        ClearSellingBlock();
    }

    public override void OnClicked(int mouseButton)
    {
        base.OnClicked(mouseButton);
    }

    public override float GetPressedTime()
    {
        return 0.1f;
    }

    public override void OnRelease(int mouseButton)
    {
        base.OnRelease(mouseButton);
        isSelling = false;
        SellingTime = 0;

        OutHover();
    }

    public override void OnHold(int mouseButton)
    {
        isSelling = true;
        if (CardsManager.singleton.SelectedCard == null)
        {
            ClearSellingBlock();
        }
    }

    public void SetTargetMove(Vector2 anchoredTargetPos, float speed)
    {
        AnchoredTargetPos = anchoredTargetPos;
        MoveToTargetSpeed = speed;
        IsMovingToTargetPos = true;
    }
}
