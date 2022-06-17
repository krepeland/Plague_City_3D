using UnityEngine;

[CreateAssetMenu(fileName = "GameResource", menuName = "GameResources/GameResourcesData", order = 0)]
public class GameResourceData : ScriptableObject
{
    public EResource EResource;
    public string ResourceName;
    public Sprite Sprite;
    public Sprite SpriteOutline;
} 
