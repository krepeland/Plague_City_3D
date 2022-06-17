using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsPack : CustomButton
{
    public CardClassType PackType;

    public List<GameCard> Cards = new List<GameCard>();
    public bool IsOpened;

    public Transform CardContainer;
    public GameObject EmptySelectorPutBack;
    public Transform CardCursorFix;

    [HideInInspector] public float TargetSize;
    [HideInInspector] public bool IsHasEmptySpace;

    private RectTransform rectTransform;
    private bool isResizingToTarget;

    private int EmptySpaceAt = -1;
    private int FirstEmptySpaceAt = -1;
    private int EmptySpaceWasAt = -1;
    private float firstPos = 0;
    private Transform UICursor;

    private bool IsMovingToTargetPos;
    private Vector2 AnchoredTargetPos = Vector2.zero;

    [HideInInspector] public float MoveToTargetSpeed = 100;
    public static float CardOffset = 90;
    public static float BordersOffset = 40;
    public static float ResizeToTargetSpeed = 1000;

    public bool IsHidden;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        RecalculateCardPosition();
        UICursor = UIManager.singleton.CursorContainer.transform;

        SetIsHidden(Cards.Count == 0);
    }

    void Update()
    {
        if (isResizingToTarget)
        {
            var dist = Mathf.Abs(rectTransform.rect.width - TargetSize);
            if (dist <= 0.1f || Time.deltaTime * ResizeToTargetSpeed / dist >= 1)
            {
                isResizingToTarget = false;
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, TargetSize);
            }
            else
            {
                rectTransform.SetSizeWithCurrentAnchors(
                    RectTransform.Axis.Horizontal,
                    Mathf.Lerp(rectTransform.rect.width, TargetSize, Time.deltaTime * ResizeToTargetSpeed / dist));
            }
        }


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
        }

        if (EmptySpaceWasAt != EmptySpaceAt) {
            RecalculateCardPosition();
            EmptySpaceWasAt = EmptySpaceAt;
        }

        if (IsHasEmptySpace) {
            CardCursorFix.transform.position = UICursor.position;
        }
    }

    public void CheckIsHidden() {
        SetIsHidden(Cards.Count == 0 && !(CardsManager.singleton.IsCardInHand && CardsManager.singleton.SelectedCard.CardClassType == PackType));
    }

    public void SetIsHidden(bool isHidden, bool isShouldSend = true)
    {
        if (BuildingsShop.singleton.IsOpened && PackType == CardClassType.Building)
        {
            IsHidden = false;
        }
        else
        {
            if (IsHasEmptySpace || BuildingsShop.singleton.IsOpened && !isHidden && PackType != CardClassType.Building)
                return;

            IsHidden = isHidden;
        }

        if (isShouldSend)
        {
            SendCardsPackWidth();
        }
        gameObject.SetActive(!IsHidden);
    }

    public void TakedCard(GameCard card, bool isTakedFromPack) {
        if (Cards.Contains(card))
        {
            EmptySpaceAt = Cards.IndexOf(card);
            if(isTakedFromPack)
                IsHasEmptySpace = true;
        }
        else
        {
            EmptySpaceAt = Cards.Count;
        }

        FirstEmptySpaceAt = EmptySpaceAt;
        EmptySelectorPutBack.SetActive(true);

        if (Cards.Contains(card))
        {
            Cards.RemoveAt(EmptySpaceAt);
        }
        RecalculateCardPosition();
    }

    public void DeletedCardFromHand() {
        EmptySpaceAt = -1;
        EmptySpaceWasAt = -1;
        FirstEmptySpaceAt = -1;
        IsHasEmptySpace = false;
        EmptySelectorPutBack.SetActive(false);
        CheckIsHidden();
    }

    public void PutCardBack(GameCard card) {
        var speed = 1000;
        if (Cards.Contains(card))
            return;

        if (!IsHasEmptySpace)
        {
            speed = 2000;
            FirstEmptySpaceAt = Mathf.Clamp(FirstEmptySpaceAt, 0, Cards.Count);
            Cards.Insert(FirstEmptySpaceAt, card);
        }
        else
        {
            EmptySpaceAt = Mathf.Clamp(EmptySpaceAt, 0, Cards.Count);
            Cards.Insert(EmptySpaceAt, card);
        }

        card.transform.SetParent(CardContainer);
        card.OutHover();
        EmptySpaceAt = -1;
        EmptySpaceWasAt = -1;
        FirstEmptySpaceAt = -1;
        IsHasEmptySpace = false;
        EmptySelectorPutBack.SetActive(false);
        RecalculateCardPosition(speed);
    }

    public void CheckWhereIsEmptySpace() {
        EmptySpaceAt = Mathf.RoundToInt((CardCursorFix.localPosition.x - firstPos) / CardOffset);
        if (EmptySpaceAt < 0)
            EmptySpaceAt = 0;
    }

    public void SetTargetSize(float targetSize)
    {
        isResizingToTarget = true;
        this.TargetSize = targetSize;
        SendCardsPackWidth();
    }

    public void SetPackTargetMove(CardsPack pack, Vector2 anchoredTargetPos, float speed)
    {
        AnchoredTargetPos = anchoredTargetPos;
        MoveToTargetSpeed = speed;
        IsMovingToTargetPos = true;
    }

    public void SendCardsPackWidth() {
        CardsManager.singleton.RecalculateCardsPackWidth(PackType, IsHidden ? 0 : TargetSize);
    }

    public void RecalculateCardPosition(int speed = 500)
    {
        for (var i = 0; i < Cards.Count; i++)
            Cards[i].transform.SetSiblingIndex(i);

        if (IsOpened)
        {
            firstPos = (Cards.Count - (IsHasEmptySpace ? 0 : 1)) / 2f * -CardOffset;
            
            var nextPos = firstPos;
            for (var i = 0; i < Cards.Count; i++)
            {
                if (IsHasEmptySpace && i == EmptySpaceAt)
                    nextPos += CardOffset;

                Cards[i].SetTargetPos(new Vector2(nextPos, 0), speed);
                nextPos += CardOffset;
            }
            SetTargetSize((-firstPos * 2) + 120 + BordersOffset);

            CardsManager.singleton.RecalculateCardsCount(PackType, Cards.Count + (IsHasEmptySpace ? 1 : 0));
        }
        else
        {
            for (var i = 0; i < Cards.Count; i++)
            {
                Cards[i].SetTargetPos(new Vector2(0, 0), speed);
            }
            SetTargetSize(120 + BordersOffset);

            CardsManager.singleton.RecalculateCardsCount(PackType, Cards.Count > 0 ? 1 : 0);
        }

    }

    public void SwitchIsOpened()
    {
        if (PackType == CardClassType.Building && BuildingsShop.singleton.IsOpened)
        {
            BuildingsShop.singleton.SwitchIsOpened();
        }
        IsOpened = !IsOpened;
        HasSwitchedIsOpened();
    }

    public void Open() {
        if (IsOpened)
            return;
        IsOpened = true;
        HasSwitchedIsOpened();
    }

    public void Close()
    {
        if (!IsOpened)
            return;
        IsOpened = false;
        HasSwitchedIsOpened();
    }

    void HasSwitchedIsOpened() {
        RecalculateCardPosition();
    }

    public override float GetPressedTime()
    {
        return 10f;
    }

    public override void OnClicked(int mouseButton)
    {
        SwitchIsOpened();
    }
}
