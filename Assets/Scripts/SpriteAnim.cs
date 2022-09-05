using UnityEngine;
using System.Collections;

public class SpriteAnim : MonoBehaviour
{
	public Texture2D frameA;
	public Texture2D frameB;

	int currentId = 0;

	bool canAnimate = false;

	void OnEnable()
	{
		canAnimate = true;
	}

	void OnDisable()
	{
		StopCoroutine("Animate");
		canAnimate = false;
	}

	void Update()
	{
		if (canAnimate)
		{
			StartCoroutine(Animate());
		}
	}

	IEnumerator Animate()
	{
		canAnimate = false;

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

		canAnimate = true;
	}
}
