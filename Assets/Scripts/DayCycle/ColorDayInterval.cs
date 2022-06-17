using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
struct SunPositions {
    public float Azimuth;
    public float Altitude;
}

public class ColorDayInterval : MonoBehaviour, ITickable
{
    [SerializeField] Gradient gradient;
    [SerializeField] Gradient skyGradient;
    [SerializeField] AnimationCurve lightIntensity;
    [SerializeField] Light lightObject;
    [SerializeField] Light SelectorLight;
    [SerializeField] Camera cameraObject;
    [SerializeField] Transform sunContainer;
    [SerializeField] List<SunPositions> sunPositions;
    [SerializeField] float AngleStart;
    [SerializeField] float AngleNow;

    [SerializeField] AnimationCurve ambientPower;
    [SerializeField] Material houseMaterial;

    int HourNow;
    int HourNext;
    SunPositions sunPositionNow;
    SunPositions sunPositionNext;

    public static ColorDayInterval singleton;

    void Awake() {
        singleton = this;
    }

    void Start()
    {
        Ticker.AddTickable(this, TickablePriority.Main);
        Tick(0);
    }

    public void Tick(int luck)
    {
        HourNow = Ticker.singleton.Hour;
        HourNext = (HourNow + 1) % 24;

        sunPositionNow = sunPositions[HourNow];
        sunPositionNext = sunPositions[HourNext];

        if (HourNow == 0) {
            sunPositionNow.Azimuth -= 360;
        }
    }

    public void Update()
    {
        var azimuth = Mathf.Lerp(sunPositionNow.Azimuth, sunPositionNext.Azimuth, Ticker.singleton.TickPercent);
        var altitude = Mathf.Lerp(sunPositionNow.Altitude, sunPositionNext.Altitude, Ticker.singleton.TickPercent);
        sunContainer.rotation = Quaternion.Euler(altitude, azimuth, 0);

        var percentNow = (HourNow + Ticker.singleton.TickPercent) / 24f;
        //AngleNow = AngleStart + percentNow * 360;
        //transform.localRotation = Quaternion.Euler(AngleNow, -30, 0);

        RenderSettings.ambientLight = gradient.Evaluate(percentNow);
        cameraObject.backgroundColor = skyGradient.Evaluate(percentNow);
        var intensity = lightIntensity.Evaluate(percentNow);
        lightObject.intensity = intensity;
        SelectorLight.intensity = (1 - intensity) * 3;

        houseMaterial.SetFloat("Vector1_2a23c27691b14b278e43f3cd776a230c", ambientPower.Evaluate(percentNow));
    }

}
