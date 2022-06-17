using UnityEngine;

[CreateAssetMenu(fileName = "UnitActionInfoData", menuName = "UnitActions/UnitActionInfoData", order = 5)]
public class UnitActionInfoData : ScriptableObject
{
    public UnitActionType ActionType;
    public Sprite Sprite;
    public Sprite SpriteOutline;
}
