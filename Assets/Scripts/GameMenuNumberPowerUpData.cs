using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuNumberPowerUpData : MonoBehaviour
{
    public bool hasSpeed;
    public bool hasShield;
    public bool hasAbracadabra;
    public int numberOfPowerUps;

    public GameMenuNumberPowerUpData(bool hasSpeed, bool hasShield, bool hasAbracadabra, int numberOfPowerUps)
    {
        this.hasSpeed = hasSpeed;
        this.hasShield = hasShield;
        this.hasAbracadabra = hasAbracadabra;
        this.numberOfPowerUps = numberOfPowerUps;
    }
}
