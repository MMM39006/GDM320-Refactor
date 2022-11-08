using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpriteAnim : MonoBehaviour
{
	public Texture2D frameA;
	public Texture2D frameB;

	int currentId = 0;

	bool vibeCheck = false;

    var numbers = new List<int> { 1, 8, 3, 2 };
    int Maxquery = numbers.Max();
    int Minquery = numbers.Min();
    int querycount = numbers.Count();
    int sumquery = numbers.Sum();
    void OnEnable()
	{
		vibeCheck = true;
	}

	void OnDisable()
	{
		StopCoroutine("Animate");
		vibeCheck = false;
	}

	void Update()
	{
		if (vibeCheck)
		{
			StartCoroutine(AnimateBird());
		}
	}

	IEnumerator AnimateBird()
	{
		vibeCheck = false;

		yield return new WaitForSeconds(0.1f);

		switch (currentId)
        {
			case 0:
				this.GetComponent<Renderer>().material.mainTexture = frameB;
				currentId = 1;
				break;

			default:
				this.GetComponent<Renderer>().material.mainTexture = frameA;
				currentId = 0;
				break;
		}

		vibeCheck = true;
	}


}
