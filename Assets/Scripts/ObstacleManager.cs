using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleManager : MonoBehaviour
{
	public List<GameObject> elements = new List<GameObject>();

	void Start()
	{
		foreach (Transform child in transform)
		{
			ElementsAdd(child);
		}
	}

	void ElementsAdd(Transform child)
    {
		if (IsShouldNotElementsAdd(child))
		{
			elements.Add(child.gameObject);
			child.gameObject.SetActive(false);
		}
	}

	bool IsShouldNotElementsAdd(Transform child)
    {
		 return child.name != "SpawnTriggerer" && child.name != "ResetTriggerer";
	}

	public void DeactivateChild()
	{
		foreach (GameObject child in elements)
		{
			Transform go = child.transform.Find("HideParticleEffect");
			GetParticleSystem(go);
			child.SetActive(false);
		}
	}

	void GetParticleSystem(Transform go)
    {
		if (go != null)
			go.GetComponent<ParticleSystem>().Stop();
	}

	public void ActivateChild()
	{
		foreach (GameObject child in elements)
		{
			child.SetActive(true);
			child.GetComponent<Renderer>().enabled = true;
			child.GetComponent<Collider>().enabled = true;
		}
	}
}
