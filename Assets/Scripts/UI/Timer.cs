using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour, ITickable
{
    [SerializeField] Text timeText;
    bool isInited;

    public void Start()
    {
        Ticker.AddTickable(this, TickablePriority.Main);
        isInited = true;
        Tick(0);
    }

    void OnEnable() {
        if (!isInited)
            return;
        Tick(0);
    }

    public void Tick(int luck)
    {
        timeText.text = $"Day {Ticker.singleton.Day}, Hour {Ticker.singleton.Hour}";
    }
}
