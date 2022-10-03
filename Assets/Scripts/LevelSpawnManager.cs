using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelSpawnManager : MonoBehaviour
{
	public BirdTrafficManager birdTrafficManager;
	public PowerUpManager powerUpManager;

	public GameObject galata;

	public float scrollSpeed = 0.4f;
	public float maxScrollSpeed = 0.7f;
	public float maxScrollSpeedDist = 1500;

	public float distance;

	public Renderer background;
	public Renderer ground;

	public List<GameObject> obstacles;

	public List<GameObject> cloudLayer;
	public List<GameObject> cityBackgroundLayer;
	public List<GameObject> cityForegroundLayer;

	static LevelSpawnManager _instance;
	static int instances = 0;

	List<GameObject> activeElements = new List<GameObject>();

	Vector2 scrolling = new Vector2(0, 0);

	float defaultScrollSpeed;
	float scrollCloudSpeed;
	float scrollBackgroundSpeed;
	float scrollForegroundSpeed;
	float scrollAtCrash;

	float galataPos = 39;

	bool IscanSpawnPowerUp = true;
	bool IsCanSpawnBirdTraffic = true;

	bool Iscanspawn = false;
	bool Ispaused = true;

	bool IsscrollGalata = true;
	bool IsCanModifySpeed = true;

	public static LevelSpawnManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(LevelSpawnManager)) as LevelSpawnManager;
			}

			return _instance;
		}
	}

	void Start()
	{
		instances++;

		WarningLevelGenerator();

		defaultScrollSpeed = scrollSpeed;
		scrollCloudSpeed = scrollSpeed * 12.5f;
		scrollBackgroundSpeed = scrollCloudSpeed * 3;
		scrollForegroundSpeed = scrollCloudSpeed * 8;

		SpawnCloudLayer(1);
		SpawnCityBackgroundLayer(1);
	}

	void WarningLevelGenerator()
    {
		if (instances > 1)
		{
			Debug.LogWarning("Warning: There are more than one Level Generator at the level");
		}
		else
		{
			_instance = this;
		}
	}

	void Update()
	{
		IscanSpawn();

		IsInvertPaused();
	}

	void IsInvertPaused()
    {
		if (!Ispaused)
		{
			distance += scrollSpeed * Time.deltaTime * 25;
			MissionManager.Instance.DistanceEvent((int)distance);
		}

	}

	void IscanSpawn()
    {
		if (Iscanspawn && !Ispaused)
        {
            IscanSpawnPowerup();
            IscanSpawnBirdTraffic();
            ScrollLevel();
        }
    }

    private void IscanSpawnBirdTraffic()
    {
		if (!IsCanSpawnBirdTraffic)
			return;
		StartCoroutine(SpawnBirdTraffic());

	}

	IEnumerator SpawnBirdTraffic()
	{
		IsCanSpawnBirdTraffic = false;

		int frequency = Random.Range(15, 35);

		if (!Ispaused)
		{
			yield return new WaitForSeconds(frequency);
		}

		if (distance < 1000)
		{
			frequency = Random.Range(10, 30);
		}
		else
		{
			frequency = Random.Range(10, 40);
		}

		frequency = frequency / 10;

		for (int i = 0; i < frequency; i++)
		{
			birdTrafficManager.LaunchBird();

			if (!Ispaused)
			{
				yield return new WaitForSeconds(1.0f);
			}

		}
		IsCanSpawnBirdTraffic = true;
	}

	private void IscanSpawnPowerup()
    {
        if (IscanSpawnPowerUp)
        {
            StartCoroutine(SpawnPowerUp());
        }
    }

	IEnumerator SpawnPowerUp()
	{
		IscanSpawnPowerUp = false;

		int frequency = Random.Range(10, 30);

		if (Ispaused)
			yield break;
		yield return new WaitForSeconds(frequency);


		powerUpManager.SpawnPowerUp(scrollSpeed / defaultScrollSpeed);
		IscanSpawnPowerUp = true;
	}

	void ScrollLevel()
    {
        IscanModifySpeed();

        scrollCloudSpeed = scrollSpeed * 12.5f;
        scrollBackgroundSpeed = scrollCloudSpeed * 3;
        scrollForegroundSpeed = scrollCloudSpeed * 8;
        LoopElenents();
        IsScrollGalata();

        scrolling.x = scrollSpeed;

        ground.material.mainTextureOffset += scrolling * Time.deltaTime;
    }

    private void IscanModifySpeed()
    {
		if (!IsCanModifySpeed)
			return;
		scrollSpeed = defaultScrollSpeed + (((maxScrollSpeedDist - (maxScrollSpeedDist - distance)) / maxScrollSpeedDist) * (maxScrollSpeed - defaultScrollSpeed));

	}

	private void LoopElenents()
	{
		for (int i = 0; i < activeElements.Count; i++)
		{
			SwitchElement(i);
		}
	}

	private void IsScrollGalata()
	{
		if (IsscrollGalata)
		{
			if (IsStopGelata())
			{
				IsscrollGalata = false;
			}
			galata.transform.position -= Vector3.right * scrollForegroundSpeed * Time.deltaTime;
		}
	}

	bool IsStopGelata()
    {
		return galata.transform.position.x < GameTransformManager.Instance.GalataEndXPosition;
	}

    void SwitchElement(int i)
    {
        SwitchElementByTag(i);
    }

    private void SwitchElementByTag(int i)
    {
        switch (activeElements[i].tag)
        {
            case "CloudLayer":
                activeElements[i].transform.position -= Vector3.right * scrollCloudSpeed * Time.deltaTime;
                break;

            case "CityBackgroundLayer":
                activeElements[i].transform.position -= Vector3.right * scrollBackgroundSpeed * Time.deltaTime;
                break;

            case "CityForegroundLayer":
            case "Obstacles":
            case "Particle":
                activeElements[i].transform.position -= Vector3.right * scrollForegroundSpeed * Time.deltaTime;
                break;
        }
    }

    void ClearMap()
	{
		StopAllCoroutines();

		birdTrafficManager.ResetAll();
		powerUpManager.ResetAll();

		while (activeElements.Count > 0)
		{
			ClearElements();
		}
	}

	void ClearElements()
    {
		switch (activeElements[0].tag)
		{
			case "CloudLayer":
				cloudLayer.Add(activeElements[0]);
				activeElements[0].transform.localPosition = new Vector3(100, activeElements[0].transform.localPosition.y, activeElements[0].transform.localPosition.z);
				activeElements[0].SetActive(false);
				activeElements.Remove(activeElements[0]);
				break;

			case "CityBackgroundLayer":
				cityBackgroundLayer.Add(activeElements[0]);
				activeElements[0].transform.localPosition = new Vector3(115, activeElements[0].transform.localPosition.y, activeElements[0].transform.localPosition.z);
				activeElements[0].SetActive(false);
				activeElements.Remove(activeElements[0]);
				break;

			case "CityForegroundLayer":
				cityForegroundLayer.Add(activeElements[0]);
				activeElements[0].transform.localPosition = new Vector3(125, activeElements[0].transform.localPosition.y, activeElements[0].transform.localPosition.z);
				activeElements[0].SetActive(false);
				activeElements.Remove(activeElements[0]);
				break;

			case "Obstacles":
				obstacles.Add(activeElements[0]);
				activeElements[0].transform.localPosition = new Vector3(175, activeElements[0].transform.localPosition.y, activeElements[0].transform.localPosition.z);
				activeElements.Remove(activeElements[0]);
				break;
		}
	}

	void RandomizeObstacles()
	{
		int n = obstacles.Count;
		GameObject temp;
		while (n > 1)
		{
			n--;
			int k = Random.Range(0, n + 1);
			temp = (GameObject)obstacles[k];
			obstacles[k] = obstacles[n];
			obstacles[n] = temp;
		}
	}


	IEnumerator StopScrolling(float time)
	{
		IsCanModifySpeed = false;

		float startValue = scrollSpeed;
		scrollAtCrash = scrollSpeed;

		//Slow down to 0 in time
		var rate = 1.0f / time;
		var t = 0.0f;

		while (t < 1.0f)
		{
			t += Time.deltaTime * rate;
			scrollSpeed = Mathf.Lerp(startValue, 0, t);
			yield return new WaitForEndOfFrame();
		}

		Iscanspawn = false;
		Ispaused = true;

		MissionManager.Instance.FailEvent((int)distance);

		yield return new WaitForSeconds(1);

		if (PlayerManager.Instance.HasRevive())
		{
			StartCoroutine(GameMenuManager.Instance.ShowRevive());
		}
		else
		{
			GameMenuManager.Instance.ShowEnd();
		}
	}

	public void SpawnCloudLayer(int placeAtLoc)
    {
        int n = Random.Range(0, cloudLayer.Count);
        GameObject randomizedCloudLayer = cloudLayer[n];

        cloudLayer.Remove(randomizedCloudLayer);
        activeElements.Add(randomizedCloudLayer);

        //If place at middle
        IsPlaceAtMiddle(placeAtLoc, randomizedCloudLayer);
        randomizedCloudLayer.SetActive(true);
    }

    private static void IsPlaceAtMiddle(int placeAtLoc, GameObject randomizedCloudLayer)
    {
        if (placeAtLoc == 1)
        {
            Vector3 newPos = randomizedCloudLayer.transform.localPosition;
            newPos.x = 0;
            randomizedCloudLayer.transform.localPosition = newPos;
        }
    }

    public void SpawnCityBackgroundLayer(int placeAtLoc)
	{
		int n = Random.Range(0, cityBackgroundLayer.Count);
		GameObject randomizedCityBackgroundLayer = cityBackgroundLayer[n];

		cityBackgroundLayer.Remove(randomizedCityBackgroundLayer);
		activeElements.Add(randomizedCityBackgroundLayer);

		//If place at middle
		IsPlacAtMiddle(placeAtLoc, randomizedCityBackgroundLayer);
		randomizedCityBackgroundLayer.SetActive(true);
	}

	void IsPlacAtMiddle(int placeAtLoc,GameObject randomizedCityBackgroundLayer)
    {
		if (placeAtLoc == 1)
		{
			Vector3 newPos = randomizedCityBackgroundLayer.transform.localPosition;
			newPos.x = 0;
			randomizedCityBackgroundLayer.transform.localPosition = newPos;
		}
	}

	public void SpawnCityForegroundLayer(int placeAtLoc)
	{
		int n = Random.Range(0, cityForegroundLayer.Count);
		GameObject randomizedCityForegroundLayer = cityForegroundLayer[n];

		cityForegroundLayer.Remove(randomizedCityForegroundLayer);
		activeElements.Add(randomizedCityForegroundLayer);

		//If place at middle
		IsPlacAtMiddle(placeAtLoc, randomizedCityForegroundLayer);
		randomizedCityForegroundLayer.SetActive(true);
	}

	public void SpawnObstacles()
	{
		int n = Random.Range(0, obstacles.Count);
		GameObject randomizedObstacle = obstacles[n];
		
		obstacles.Remove(randomizedObstacle);
		activeElements.Add(randomizedObstacle);
		Debug.Log(randomizedObstacle);
		randomizedObstacle.GetComponent<ObstacleManager>().ActivateChild();
	}

	public void PoolGameObject(GameObject pooledObject)
	{
		PoolingObjectCase( pooledObject);

		if (pooledObject.tag != "Obstacles")
		{
			pooledObject.SetActive(false);
		}
	}

	void PoolingObjectCase(GameObject pooledObject)
    {
		switch (pooledObject.tag)
		{
			case "CloudLayer":
				activeElements.Remove(pooledObject);
				cloudLayer.Add(pooledObject);
				pooledObject.transform.localPosition = new Vector3(100, pooledObject.transform.localPosition.y, pooledObject.transform.localPosition.z);
				break;

			case "CityBackgroundLayer":
				activeElements.Remove(pooledObject);
				cityBackgroundLayer.Add(pooledObject);
				pooledObject.transform.localPosition = new Vector3(115, pooledObject.transform.localPosition.y, pooledObject.transform.localPosition.z);
				break;

			case "CityForegroundLayer":
				activeElements.Remove(pooledObject);
				cityForegroundLayer.Add(pooledObject);
				pooledObject.transform.localPosition = new Vector3(125, pooledObject.transform.localPosition.y, pooledObject.transform.localPosition.z);
				break;

			case "Obstacles":
				activeElements.Remove(pooledObject);
				obstacles.Add(pooledObject);
				pooledObject.transform.localPosition = new Vector3(175, pooledObject.transform.localPosition.y, pooledObject.transform.localPosition.z);
				break;

		}
	}

	public void AddParticle(GameObject exp)
	{
		activeElements.Add(exp);
	}

	public void RemoveParticle(GameObject exp)
	{
		activeElements.Remove(exp);
	}

	public void Restart(bool startToScroll)
	{
		ClearMap();
		GameMenuManager.Instance.mainMenuElements[10].SetActive(true);
		galata.transform.localPosition = new Vector3(galataPos, 1, galata.transform.localPosition.z);

		IsCanSpawnBirdTraffic = true;
		IscanSpawnPowerUp = true;
		IsCanModifySpeed = true;

		distance = 0;
		scrollSpeed = defaultScrollSpeed;

		SpawnCloudLayer(1);
		SpawnCityBackgroundLayer(1);

		IsscrollGalata = true;

		if (!startToScroll)
			return;
		StartCoroutine(StartToSpawn(1.25f, 3));

	}

	public void Pause()
	{
		Iscanspawn = false;
		Ispaused = true;

		birdTrafficManager.PauseAll();
		powerUpManager.PauseAll();
	}

	public void Resume()
	{
		Iscanspawn = true;
		Ispaused = false;

		birdTrafficManager.ResumeAll();
		powerUpManager.ResumeAll();
	}

	public void ExtraSpeedEffect()
	{
		IsCanModifySpeed = false;
	}

	public void ExtraSpeedOver()
	{
		IsCanModifySpeed = true;
	}

	public void ContinueScrolling()
	{
		scrollSpeed = scrollAtCrash;

		Ispaused = false;
		Iscanspawn = true;
		IsCanModifySpeed = true;
	}

	public float SpeedMultiplier()
	{
		return scrollSpeed / defaultScrollSpeed;
	}

	public void SetGalataPos(float pos)
	{
		galataPos = pos;
	}

	public IEnumerator ScrollExplosion(ParticleSystem particle)
	{
		GameObject particleGO = particle.gameObject;
		activeElements.Add(particleGO);

		if (!Ispaused)
		{
			yield return new WaitForSeconds(2f);
		}

		activeElements.Remove(particleGO);
	}

	public IEnumerator StartToSpawn(float waitTime, float obstacleWaitTime)
	{
		RandomizeObstacles();

		Iscanspawn = true;
		Ispaused = false;

		if (!Ispaused)
		{
			yield return new WaitForSeconds(waitTime);
		}

		SpawnCityForegroundLayer(0);
	}
}
