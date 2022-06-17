using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Cards/CardData", order = 0)]
public class CardData : ScriptableObject
{
    public string CardName;
    public Sprite CardSprite;

    public CardClassType CardClassType;
    public ESelectorMode SelectorModeWhenTaked;

    [Multiline]
    public string Description;
}
