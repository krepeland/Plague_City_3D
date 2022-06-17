using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutBack : CustomButton
{
    bool IsHovering;
    public CardsPack cardsPack;
    public static Dictionary<CardClassType, PutBack> PutBacks = new Dictionary<CardClassType, PutBack>();

    private void Awake()
    {
        PutBacks[cardsPack.PackType] = this;
    }

    public override void OnHover()
    {
        IsHovering = true;
        cardsPack.IsHasEmptySpace = true;
        cardsPack.RecalculateCardPosition();

        var selectedCard = CardsManager.singleton.SelectedCard;
        if (selectedCard == null || selectedCard.CardClassType != cardsPack.PackType)
            return;

        selectedCard.transform.localPosition = new Vector3(0, 0, 0);
    }

    public override void OutHover()
    {
        IsHovering = false;
        cardsPack.IsHasEmptySpace = false;
        cardsPack.RecalculateCardPosition();

        var selectedCard = CardsManager.singleton.SelectedCard;
        if (selectedCard == null || selectedCard.CardClassType != cardsPack.PackType)
            return;
        selectedCard.transform.localPosition = new Vector3(130, -120, 0);
    }

    void Update()
    {
        if (IsHovering) {
            cardsPack.CheckWhereIsEmptySpace();
        }
    }
}
