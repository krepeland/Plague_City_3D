using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TasksDisplay : CustomButton
{
    public RectTransform MainTasksText;
    public RectTransform ExtraTasksText;
    public RectTransform TargetRect;
    public RectTransform LineRect;
    public RectTransform MaskRect;

    [SerializeField] RectTransform HideButtonTextRect;

    public List<Image> BackImgaes = new List<Image>();

    bool isOpened;
    bool isChanging;

    float T;
    float closedHeight;
    float openedHeight;

    public static TasksDisplay singleton;

    public List<Text> mainTexts;
    public List<Text> extraTexts;

    int MainTasksCount;
    int ExtraTasksCount;

    private void Awake()
    {
        singleton = this;

        openedHeight = 40;
        closedHeight = 40;
    }

    public void Init(int mainTasksCount, int extraTasksCount) 
    {
        MainTasksCount = mainTasksCount;
        ExtraTasksCount = extraTasksCount;

        var prefabMainText = PrefabManager.GetPrefab("UI_MainTask");
        for (var i = 1; i <= MainTasksCount; i++)
        {
            var task = Instantiate(prefabMainText, MaskRect);
            task.GetComponent<RectTransform>().anchoredPosition = new Vector2(5, -25 * i);
            mainTexts.Add(task.GetComponent<Text>());
        }

        openedHeight = 40 + 25 * MainTasksCount;
        if (ExtraTasksCount > 0)
        {
            var prefabExtraText = PrefabManager.GetPrefab("UI_ExtraTask");
            for (var i = 1; i <= ExtraTasksCount; i++)
            {
                var task = Instantiate(prefabExtraText, MaskRect);
                task.GetComponent<RectTransform>().anchoredPosition = new Vector2(42, -openedHeight + 3 - 25 * i);
                extraTexts.Add(task.GetComponent<Text>());
            }

            LineRect.anchoredPosition = new Vector2(5, -openedHeight + 8);
            ExtraTasksText.anchoredPosition = new Vector2(42, -openedHeight + 3);
            openedHeight += 35 + 25 * ExtraTasksCount;
        }
    }

    public void UpdateMainTaskText(int index, string text) 
    {
        mainTexts[index].text = text;
    }

    public void UpdateExtraTaskText(int index, string text)
    {
        extraTexts[index].text = text;
    }

    private void OnEnable()
    {
        isChanging = true;
        isOpened = true;
        T = 0;
    }

    private void Update()
    {
        if (isChanging)
        {
            T += (isOpened ? Time.deltaTime : -Time.deltaTime) * 2;
            if (T > 1 || T < 0) {
                T = Mathf.Clamp01(T);
                isChanging = false;
            }
            TargetRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(closedHeight, openedHeight, T));

            HideButtonTextRect.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(270, 90, T));
        }
    }

    public void ForceOpen() {
        isChanging = true;
        isOpened = true;
        T = 1;
        Update();
    }

    public void ChangeIsOpened() {
        isOpened = !isOpened;

        //HideButtonTextRect.transform.localRotation = Quaternion.Euler(0, 0, isOpened ? 90 : -90);
        isChanging = true;
    }

    public override void OnClicked(int mouseButton)
    {
        base.OnClicked(mouseButton);
        ChangeIsOpened();
    }

    public override float GetPressedTime()
    {
        return 10f;
    }
}
