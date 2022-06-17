using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlurFix : MonoBehaviour
{
    [SerializeField] bool isRect = true;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] RectTransform targetRectTransform;
    [SerializeField] bool isSizeDelta;

    [SerializeField] bool isColor;
    [SerializeField] Image image;
    [SerializeField] Image targetImage;

    [SerializeField] bool isEnabled;
    [SerializeField] GameObject targetObject;

    void OnGUI()
    {
        if (isRect)
        {
            rectTransform.position = targetRectTransform.position;
            rectTransform.localScale = targetRectTransform.localScale;
        }

        if(isSizeDelta)
            rectTransform.sizeDelta = targetRectTransform.sizeDelta;

        if (isColor)
            image.color = targetImage.color;

        if (isEnabled)
            image.color = targetObject.activeSelf ? Color.white : Color.clear;
    }
}
