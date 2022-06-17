using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMapInfo : MonoBehaviour
{
    public Text InfoText;
    Vector2Int selectedLevelCoords;

    public static LevelMapInfo singleton;

    private void Awake()
    {
        singleton = this;
        gameObject.SetActive(false);
    }

    void SetLevelData(LevelData level)
    {
        var size = level.MainTasks.Count * 25 + level.ExtraTasks.Count * 25 + 50;
        var resultText = "MAIN TASKS:\n";
        foreach (var e in level.MainTasks) {
            resultText += $"O  {e.GetTaskInMenuString()}\n";
        }
        if (level.ExtraTasks.Count > 0)
        {
            size += 25;
            resultText += "\tEXTRA TASKS:\n";
            foreach (var e in level.ExtraTasks)
            {
                resultText += $"\tO  {e.GetTaskInMenuString()}\n";
            }
        }
        InfoText.text = resultText;
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
        gameObject.SetActive(true);
    }

    public void Close() {
        gameObject.SetActive(false);
    }

    public void LoadLevelData(Vector2Int levelCoords)
    {
        if (!LevelMap.singleton.LevelsOnCoords.ContainsKey(levelCoords))
            return;
        selectedLevelCoords = levelCoords;
        SetLevelData(LevelMapManager.LevelsDataPoses[levelCoords]);
    }

    public void LoadLevel()
    {
        LevelMapManager.LoadLevel(selectedLevelCoords);
    }

    void Update()
    {
        
    }
}
