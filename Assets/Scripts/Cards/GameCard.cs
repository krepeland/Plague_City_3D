using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCard : CustomButton
{
    public CardData CardData { get; private set; }
    public CardClassType CardClassType;
    static float PressedTime = 0.1f;

    private bool IsMovingToTargetPos;
    private Vector2 AnchoredTargetPos = Vector2.zero;
    private float MoveToTargetSpeed = 100;
    
    RectTransform rectTransform;

    [HideInInspector] public CardsPack cardsPack;
    public Text CardName;
    public Image CardImage;
    public Image CardStamina;

    float staminaMax;
    float staminaNow;

    private float fixRestTime;

    private bool isHovered;
    private float hoveredTime;

    void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetCardData(CardData cardData) {
        CardData = cardData;
        
        CardClassType = cardData.CardClassType;
        cardsPack = CardsManager.singleton.CardsPacks[CardClassType];
        CardsManager.singleton.AddCardToDeck(this);
        transform.localPosition = Vector2.zero;
        transform.localRotation = Quaternion.Euler(0, 0, 0);

        CardName.text = cardData.CardName.ToUpper();

        if (CardData.CardSprite != null)
        {
            CardImage.color = Color.white;
            CardImage.sprite = cardData.CardSprite;
        }
        else{
            CardImage.color = new Color(0.31f, 0.31f, 0.31f, 1); 
        }
    }

    public void SetStamina(float staminaMax, float staminaNow) {
        this.staminaMax = staminaMax;
        this.staminaNow = staminaNow;
    }

    public void CardTakedInHand(int takedWay)
    {
        isHovered = false;
        hoveredTime = 0;
        BuyBuildingInfo.singleton.SetVisible(false);
        CardsManager.singleton.TakedCard(this, takedWay);
    }

    void Update() {
        if (!Ticker.IsPaused)
        {
            staminaNow -= Time.deltaTime / Ticker.singleton.SecondsPerTick;
        }
        if (staminaNow <= 0)
        {
            CardStamina.fillAmount = 0;
        }
        else
        {
            CardStamina.fillAmount = staminaNow / staminaMax;
        }

        if (IsMovingToTargetPos) {
            var dist = Vector2.Distance(rectTransform.anchoredPosition, AnchoredTargetPos);
            if (dist <= 0.1f || Time.deltaTime * MoveToTargetSpeed / dist >= 1)
            {
                IsMovingToTargetPos = false;
                rectTransform.anchoredPosition = AnchoredTargetPos;
            }
            else {
                rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, AnchoredTargetPos, Time.deltaTime * MoveToTargetSpeed / dist);
            }
        }

        if (isHovered)
        {
            hoveredTime += Time.deltaTime;
            if (hoveredTime > 0.35f) {
                isHovered = false;
                BuyBuildingInfo.singleton.SetVisible(true);
                BuyBuildingInfo.singleton.SetInfo(CardsManager.singleton.GetBuyCardDataOrNull(CardData.CardName), CardData, transform, false);
            }
        }
    }

    public void SetTargetPos(Vector2 anchoredTargetPos, float speed) {
        IsMovingToTargetPos = true;
        AnchoredTargetPos = anchoredTargetPos;
        MoveToTargetSpeed = speed;
    }

    public override void OnClicked(int mouseButton)
    {
        if (staminaNow <= 0)
        {
            CardTakedInHand(0);
        }

        transform.localScale = Vector3.one;
    }

    public override void OnHover()
    {
        if (staminaNow <= 0)
            transform.localScale = Vector3.one * 1.2f;
        isHovered = true;
    }

    public override void OutHover()
    {
        transform.localScale = Vector3.one;
        isHovered = false;
        hoveredTime = 0;
        BuyBuildingInfo.singleton.SetVisible(false);
    }

    public override void OnHold(int mouseButton)
    {
        if (staminaNow <= 0)
        {
            CardTakedInHand(1);
        }

        transform.localScale = Vector3.one;
    }

    public override void OnRelease(int mouseButton)
    {
        transform.localScale = Vector3.one;
        if (CardsManager.singleton.SelectedCard == this) {
            PutBack.PutBacks[CardClassType].OutHover();
        }
    }

    public override float GetPressedTime()
    {
        return PressedTime;
    }

    private void OnDisable()
    {
        fixRestTime = Ticker.singleton.Ticks + Ticker.singleton.TickPercent;
    }

    private void OnEnable()
    {
        staminaNow -= (Ticker.singleton.Ticks + Ticker.singleton.TickPercent - fixRestTime);
    }
}
