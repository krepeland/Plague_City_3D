using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    public Vector2Int PosOnMap;
    public LevelData levelData;
    [SerializeField] List<Image> DifficultyStars;
    [SerializeField] Color ActiveStar;
    [SerializeField] Color DisactiveStar;

    [SerializeField] Image IslandImage;
    [SerializeField] Color LockedColor;
    [SerializeField] Color UnlockedColor;
    [SerializeField] Color FailedColor;
    [SerializeField] Color CompletedColor;

    void SetDifficultyStart(int difficulty) {
        for (var i = 1; i < 4; i++) {
            DifficultyStars[i - 1].color = i > difficulty ? DisactiveStar : ActiveStar;
        }
    }

    public void InitLevel(LevelData data, Vector2Int posOnMap)
    {
        levelData = data;
        PosOnMap = posOnMap;
        SetDifficultyStart(levelData.Size / 3 + 1);
    }

    public void UpdatedLevelStatus(LevelStatus levelStatus) {
        levelData.LevelStatus = levelStatus;
        switch (levelStatus) {
            case LevelStatus.Locked:
                GetComponent<Button>().interactable = false;
                IslandImage.color = LockedColor;
                break;
            case LevelStatus.Unlocked:
                GetComponent<Button>().interactable = true;
                IslandImage.color = UnlockedColor;
                break;
            case LevelStatus.Failed:
                GetComponent<Button>().interactable = false;
                IslandImage.color = FailedColor;
                break;
            case LevelStatus.Completed:
                GetComponent<Button>().interactable = false;
                IslandImage.color = CompletedColor;
                break;
        }
    }

    public void Pressed()
    {
        LevelMap.singleton.LevelSelected(this);
    }

    public void Selected()
    {
        LevelMapInfo.singleton.LoadLevelData(PosOnMap);
    }

    public void Unselected()
    {

    }
}
