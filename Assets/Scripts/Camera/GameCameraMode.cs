using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraMode : MonoBehaviour
{
    public static GameCameraMode singleton;
    public static EGameCameraMode CameraMode;

    public float ShowSpecialValue;
    public AnimationCurve InfectionShowCurve;
    
    float ShowSpecialTime;
    float SwapValueTime;
    public CameraModeCircleSelector CircleSelector;
    public Texture2D TextureNow;

    private void Awake()
    {
        singleton = this;
        CameraMode = EGameCameraMode.Normal;
    }

    private void Start()
    {
        CircleSelector = CameraModeCircleSelector.singleton;
    }

    public void SwapGameCameraMode(EGameCameraMode cameraMode) {
        if (CameraMode == cameraMode)
        {
            SetGameCameraMode(EGameCameraMode.Normal);
            return;
        }

        SetGameCameraMode(cameraMode);
    }

    public void NextGameCameraMode()
    {
        switch (CameraMode) {
            case EGameCameraMode.Normal:
                SetGameCameraMode(EGameCameraMode.Infection);
                break;
            case EGameCameraMode.Infection:
                SetGameCameraMode(EGameCameraMode.Normal);
                break;
            case EGameCameraMode.Resources:
                SetGameCameraMode(EGameCameraMode.Normal);
                break;
            case EGameCameraMode.Discontent:
                SetGameCameraMode(EGameCameraMode.Normal);
                break;
        }
    }

    public void SelectedGameCameraMode() {
        switch (CircleSelector.IndexNow) {
            case 0:
                SetGameCameraMode(EGameCameraMode.Normal);
                break;
            case 1:
                SetGameCameraMode(EGameCameraMode.Infection);
                break;
            case 2:
                SetGameCameraMode(EGameCameraMode.Resources);
                break;
            case 3:
                SetGameCameraMode(EGameCameraMode.Discontent);
                break;
        }
        CircleSelector.SetIsVisible(false);
    }

    public void SetGameCameraMode(EGameCameraMode cameraMode) {
        CameraMode = cameraMode;
        if (TextureNow == null)
            TextureNow = InfectionManager.InfectionTexture;

        SwapValueTime = 0;
        Shader.SetGlobalTexture("Texture2D_fb2a2b145c174ad49741b6c093d5da83", TextureNow);

        switch (cameraMode) {
            case EGameCameraMode.Infection:
                TextureNow = InfectionManager.InfectionTexture;
                break;
            case EGameCameraMode.Resources:
                TextureNow = ResourceManager.FeedTexture;
                break;
            case EGameCameraMode.Discontent:
                TextureNow = DiscontentManager.DiscontentTexture;
                break;
        }

        Shader.SetGlobalTexture("Texture2D_0f712be9b4cf48069a6642e8cd337aa9", TextureNow);
    }

    public void SetGameCameraMode(int cameraMode)
    {
        SetGameCameraMode((EGameCameraMode)cameraMode);
    }

    void Update()
    {
        SwapValueTime = Mathf.Clamp01(SwapValueTime + Time.deltaTime * 5);

        if (CameraMode == EGameCameraMode.Normal)
        {
            ShowSpecialTime -= Time.deltaTime;
        }
        else
        {
            ShowSpecialTime += Time.deltaTime;
        }

        ShowSpecialTime = Mathf.Clamp(ShowSpecialTime, 0, 1.25f);
        ShowSpecialValue = InfectionShowCurve.Evaluate(ShowSpecialTime * 0.8f) * 20;

        Shader.SetGlobalFloat("Vector1_cdd4a769a1384aafa551ca41a4f9d824", ShowSpecialValue);
        Shader.SetGlobalFloat("Vector1_aac5af4b1936497f8845489b55c7b3d7", SwapValueTime);
    }
}
