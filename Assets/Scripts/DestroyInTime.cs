using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInTime : MonoBehaviour
{
    public float LifeTime;
    float T;
    void Update()
    {
        T += Time.deltaTime;
        if (T >= LifeTime) {
            Destroy(gameObject);
        }
    }
}
