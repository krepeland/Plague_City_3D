using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

enum GameRoundEndStage { 
    Inited,
    AppearBlur,
    MoveTasks,
    Rewards,
    AppearContinueButton,
    GameEnded,
}

public class GameRoundEnd : MonoBehaviour
{
    public static GameRoundEnd singleton;

    public Image backImage;
    public Material EndBlurMaterial;

    [SerializeField] GameRoundEndStage stage;
    float T;
    [SerializeField] RectTransform panelTransformContainer;
    [SerializeField] RectTransform panelTransform;
    List<Text> mainGameTasksTexts;
    List<TaskStatus> mainTasksStatus;
    List<Text> extraGameTasksTexts;
    List<TaskStatus> extraTasksStatus;

    List<(RectTransform, Vector2)> allTaskRects;

    RectTransform mainTasksText;
    RectTransform extraTasksText;

    List<Image> tasksImagesToHide;

    [SerializeField] List<Image> ImagesToAppear;
    List<(Image, Color, Color)> ImagesToAppearColors;
    [SerializeField] List<Text> TextesToAppear;
    List<(Text, Color, Color)> TextesToAppearColors;
    int indexNow;
    bool rewardFlag;
    int coins;
    float coinDeltaY;
    [SerializeField] Transform CoinContainer;
    Transform CoinPrefab;
    [SerializeField] Text ResultText;
    bool IsWin;
    [SerializeField] RectTransform ContinueButton;

    public static int CoinsCount = 0;

    public void Init()
    {
        singleton = this;
        EndBlurMaterial.SetFloat("Vector1_32711c4695b648ddac39dd9766679a05", 0);
        backImage.color = new Color(0, 0, 0, 0);
        stage = GameRoundEndStage.Inited;

        gameObject.SetActive(false);

        CoinPrefab = PrefabManager.GetPrefab("UI_Coin");
    }

    public void GameEnded(bool isWin)
    {
        if (stage == GameRoundEndStage.Inited)
        {
            CoinsCount = 0;
            IsWin = isWin;
            TasksDisplay.singleton.ForceOpen();

            gameObject.SetActive(true);
            stage = GameRoundEndStage.AppearBlur;

            panelTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, TasksDisplay.singleton.TargetRect.sizeDelta.y);

            allTaskRects = new List<(RectTransform, Vector2)>();

            mainGameTasksTexts = TasksDisplay.singleton.mainTexts;
            mainTasksStatus = TaskManager.MainTasksStatus;

            extraGameTasksTexts = TasksDisplay.singleton.extraTexts;
            extraTasksStatus = TaskManager.ExtraTasksStatus;

            mainTasksText = TasksDisplay.singleton.MainTasksText;
            allTaskRects.Add((mainTasksText, mainTasksText.anchoredPosition));
            mainTasksText.transform.SetParent(panelTransformContainer);

            foreach (var e in mainGameTasksTexts)
            {
                var r = e.GetComponent<RectTransform>();
                allTaskRects.Add((r, r.anchoredPosition));
                e.transform.SetParent(panelTransformContainer);
            }

            if (extraGameTasksTexts.Count > 0)
            {
                extraTasksText = TasksDisplay.singleton.ExtraTasksText;
                allTaskRects.Add((extraTasksText, extraTasksText.anchoredPosition));
                extraTasksText.transform.SetParent(panelTransformContainer);

                foreach (var e in extraGameTasksTexts)
                {
                    var r = e.GetComponent<RectTransform>();
                    allTaskRects.Add((r, r.anchoredPosition));
                    e.transform.SetParent(panelTransformContainer);
                }
            }
            TasksDisplay.singleton.transform.SetParent(panelTransformContainer);
            TasksDisplay.singleton.enabled = false;
            TasksDisplay.singleton.GetComponent<RectTransform>().SetSiblingIndex(0);
            tasksImagesToHide = TasksDisplay.singleton.BackImgaes;

            ImagesToAppearColors = new List<(Image, Color, Color)>();
            foreach (var e in ImagesToAppear)
            {
                var c = new Color(e.color.r, e.color.g, e.color.b, 0);
                ImagesToAppearColors.Add((e, c, new Color(e.color.r, e.color.g, e.color.b, e.color.a)));
                e.color = c;
            }
            TextesToAppearColors = new List<(Text, Color, Color)>();
            foreach (var e in TextesToAppear)
            {
                var c = new Color(e.color.r, e.color.g, e.color.b, 0);
                TextesToAppearColors.Add((e, c, new Color(e.color.r, e.color.g, e.color.b, e.color.a)));
                e.color = c;
            }

            coinDeltaY = TasksDisplay.singleton.TargetRect.sizeDelta.y / ((float)mainTasksStatus.Count + extraTasksStatus.Count);

            ResultText.text = isWin ? "VICTORY" : "DEFEAT";
        }
    }

    private void Update()
    {
        switch (stage) {
            case GameRoundEndStage.AppearBlur:
                CameraController.singleton.zoomValue = 80;
                CameraController.singleton.UpdateZoom(2);
                T = Mathf.Clamp01(T + Time.deltaTime);
                EndBlurMaterial.SetFloat("Vector1_32711c4695b648ddac39dd9766679a05", Mathf.Lerp(0, 0.005f, T));
                backImage.color = new Color(0, 0, 0, Mathf.Lerp(0, 0.4f, T));

                foreach (var e in tasksImagesToHide)
                    e.color = new Color(e.color.r, e.color.g, e.color.b, Mathf.Lerp(1, 0, T));
                foreach (var e in ImagesToAppearColors)
                    e.Item1.color = Color.Lerp(e.Item2, e.Item3, T);
                foreach (var e in TextesToAppearColors)
                    e.Item1.color = Color.Lerp(e.Item2, e.Item3, T);

                if (T >= 1) {
                    stage = GameRoundEndStage.MoveTasks;
                    foreach (var e in tasksImagesToHide)
                    {
                        e.gameObject.SetActive(false);
                    }
                    T = 0;
                }
                return;
            case GameRoundEndStage.MoveTasks:
                T += Time.deltaTime;
                CameraController.singleton.UpdateZoom(2);
                for (var i = 0; i < allTaskRects.Count; i++) {
                    if (i > T * 5f)
                        break;
                    var r = allTaskRects[allTaskRects.Count - 1 - i];
                    var dist = Vector2.Distance(r.Item1.anchoredPosition, r.Item2);
                    var value = 0f;
                    if (dist <= 0)
                        value = 1;
                    else
                    {
                        value = Time.deltaTime / dist * 500;
                        if (value >= 1)
                        {
                            value = 1;
                            if (i == allTaskRects.Count - 1) {
                                stage = GameRoundEndStage.Rewards;
                                T = 0;
                            }
                        }
                    }
                    r.Item1.anchoredPosition = Vector2.Lerp(r.Item1.anchoredPosition, r.Item2, value);
                }
                return;
            case GameRoundEndStage.Rewards:
                CameraController.singleton.UpdateZoom(2);
                T += Time.deltaTime * 2;
                if (T >= 1) {
                    indexNow += 1;
                    T = 0;
                    rewardFlag = false;
                }

                if (indexNow > mainTasksStatus.Count + extraTasksStatus.Count - 1)
                {
                    T = 0;
                    stage = GameRoundEndStage.AppearContinueButton;
                    return;
                }

                if (indexNow < mainTasksStatus.Count)
                {
                    mainGameTasksTexts[indexNow].transform.localScale = Vector3.one * (1 + 0.2f * Mathf.Sin(Mathf.PI * T));
                    if (IsWin && mainTasksStatus[indexNow] == TaskStatus.Completed)
                    {
                        if (!rewardFlag)
                        {
                            rewardFlag = true;
                            SpawnCoin(mainGameTasksTexts[indexNow]);

                            if (GameManager.LevelData != null)
                                CoinsCount += GameManager.LevelData.Difficulty;
                            else
                                CoinsCount += 1;
                        }
                    }
                    else
                    {
                        if (!rewardFlag)
                        {
                            rewardFlag = true;
                            mainGameTasksTexts[indexNow].text = mainGameTasksTexts[indexNow].text.Replace("red", "grey");
                            mainGameTasksTexts[indexNow].text = mainGameTasksTexts[indexNow].text.Replace("#ffffff", "grey");
                            mainGameTasksTexts[indexNow].text = mainGameTasksTexts[indexNow].text.Replace("#8AD68D", "grey");
                        }
                        //mainGameTasksTexts[indexNow].transform.localScale = new Vector3(1 - Mathf.Sin(Mathf.PI * T * 0.5f), 1, 1);
                    }
                }
                else {
                    var i = indexNow - mainTasksStatus.Count;
                    extraGameTasksTexts[i].transform.localScale = Vector3.one * (1 + 0.2f * Mathf.Sin(Mathf.PI * T));
                    if (IsWin && extraTasksStatus[i] == TaskStatus.Completed)
                    {
                        if (!rewardFlag)
                        {
                            rewardFlag = true;
                            SpawnCoin(extraGameTasksTexts[i]);

                            if (GameManager.LevelData != null)
                                CoinsCount += GameManager.LevelData.Difficulty;
                            else
                                CoinsCount += 1;
                        }
                    }
                    else
                    {
                        if (!rewardFlag)
                        {
                            rewardFlag = true;
                            extraGameTasksTexts[i].text = extraGameTasksTexts[i].text.Replace("red", "grey");
                            extraGameTasksTexts[i].text = extraGameTasksTexts[i].text.Replace("#ffffff", "grey");
                            extraGameTasksTexts[i].text = extraGameTasksTexts[i].text.Replace("#8AD68D", "grey");
                        }
                        //extraGameTasksTexts[i].transform.localScale = new Vector3(1 - Mathf.Sin(Mathf.PI * T * 0.5f), 1, 1);
                    }
                }
                return;
            case GameRoundEndStage.AppearContinueButton:
                CameraController.singleton.UpdateZoom(2);
                T += Time.deltaTime * 2;
                if (T >= 1) {
                    T = 1;
                }
                ContinueButton.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(0, 60, T));

                if (T >= 1) {
                    stage = GameRoundEndStage.GameEnded;
                    T = 0;
                }
                return;
        }
    }

    void SpawnCoin(Text text)
    {
        coins += 1;
        var coin = Instantiate(CoinPrefab, CoinContainer);
        coin.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, coinDeltaY * (coins - 0.5f));
    }

    public void ContinueButtonPressed()
    {
        var e = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }
}
