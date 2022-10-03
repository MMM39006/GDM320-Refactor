using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveToPositionData : MonoBehaviour
{
    public Transform obj;
    public Vector3 endPos;
    public float time;
    public bool enableControls;

    public PlayerMoveToPositionData(Transform obj, Vector3 endPos, float time, bool enableControls)
    {
        this.obj = obj;
        this.endPos = endPos;
        this.time = time;
        this.enableControls = enableControls;
    }
}
