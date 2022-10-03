using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnData : MonoBehaviour
{
    public TextMesh target;
    public int data;
    public int digitNumbers;

    public SpawnData (TextMesh target, int data, int digitNumbers)
    {
        this.target = target;
        this.data = data;
        this.digitNumbers = digitNumbers;
    }
}
