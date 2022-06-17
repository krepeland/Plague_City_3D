using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscontentManager : MonoBehaviour
{
    public static DiscontentManager singleton;
    public static Texture2D DiscontentTexture;

    public static int DiscontentForHungerTick = 3;
    public static int DiscontentForInfected = 1;
    public static int DiscontentForDeadInfected = 5;
    public static int DiscontentForDeadHealthy = 15;


    private static int _maxDiscontent;
    public static int MaxDiscontent
    {
        get => _maxDiscontent;
        set
        {
            _maxDiscontent = value;
            TaskManager.UpdatedETask(ETask.DiscontentMedium);
            //Debug.Log("Max discontent: " + _maxDiscontent);
        }
    }

    private static int _totalDiscontent;

    public static int TotalDiscontent
    {
        get => _totalDiscontent;
        set
        {
            _totalDiscontent = value;
            TaskManager.UpdatedETask(ETask.DiscontentMedium);
            //Debug.Log("Total discontent: " + _totalDiscontent);
        }
    }

    private void Awake()
    {
        _totalDiscontent = 0;
        _maxDiscontent = 0;
        singleton = this;

        DiscontentTexture = new Texture2D(80, 80);
        DiscontentTexture.filterMode = FilterMode.Point;
        DiscontentTexture.wrapMode = TextureWrapMode.Clamp;
        for (var x = 0; x < 80; x++)
        {
            for (var y = 0; y < 80; y++)
            {
                SetDiscontentTextureState(x, y, 0, false);
            }
        }
    }

    public static void SetDiscontentTextureState(int x, int y, int discontent, bool isEmpty)
    {
        //var value = Mathf.Clamp(discontent - 20, 0, 80) * 0.0125f;
        //var color = Color.LerpUnclamped(Color.white, Color.red, value);

        var value = (discontent / 10) * 0.1f;
        var color = Color.LerpUnclamped(Color.white, Color.red, value);
        if (isEmpty)
            color = Color.black;
        DiscontentTexture.SetPixel(x, y, color);
        DiscontentTexture.Apply();
    }
}
