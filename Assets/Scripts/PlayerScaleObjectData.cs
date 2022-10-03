using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScaleObjectData : MonoBehaviour
{
    public Transform obj;
    public Vector3 scale;
    public float time;
    public bool deactivate;

    public PlayerScaleObjectData (Transform obj, Vector3 scale, float time, bool deactivate)
    {
        this.obj = obj;
        this.time = time;
        this.deactivate = deactivate;
    }
}
