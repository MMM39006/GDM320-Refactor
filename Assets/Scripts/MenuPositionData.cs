using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPositionData : MonoBehaviour
{
    public bool speed;
    public bool shield;
    public bool abracadabra;
    public float positionMove;
    public bool Ishid;

    public MenuPositionData(bool speed, bool shield, bool abracadabra, float positionMove, bool Ishid)
    {
        this.speed = speed;
        this.shield = shield;
        this.abracadabra = abracadabra;
        this.positionMove = positionMove;
        this.Ishid = Ishid;
    }
}
