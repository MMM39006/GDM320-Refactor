using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrashEffectData : MonoBehaviour
{
    public float crashPosY;
    public float crashPosEdge;
    public float distance;

    public PlayerCrashEffectData(float crashPosY, float crashPosEdge, float distance)
    {
        this.crashPosY = crashPosY;
        this.crashPosEdge = crashPosEdge;
        this.distance = crashPosEdge;
    }
}
