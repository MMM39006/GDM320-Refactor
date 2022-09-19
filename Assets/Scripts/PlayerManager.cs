using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
	public GameObject startAnimation;

	public GameObject horizontalAnimation;

	private Animator characterAnimator;

	public SphereCollider shieldCollider;
	public GameObject speedParticle;
	public GameObject speedTrail;
	public GameObject abracadabra;

	public float topEdge = 28f;
	public float bottomEdge = -5f;
	public float maxRotation = 25f;
	public float maxVerticalSpeed = 55.0f;
	public float safeEdgeZone = 15.0f;

	public ParticleSystem smoke;
	public ParticleSystem reviveParticle;

	static PlayerManager _instance;
	static int instances = 0;

	float speed = 0.0f;
	float newSpeed = 0.0f;

	float rotationFactor;
	Vector3 newRotation = new Vector3(0, 0, 0);


	float distanceToTop;
	float distanceToBottom;

	float xPos = -30;
	float startingPos = -37;

	bool IsmovingUpward = false;
	bool IscontrolEnabled = false;
	bool IsCrash = true;
	bool Iscrashing = false;
	bool Iscrashed = false;
	public bool IsfirstObstacleSpawned = false;
	bool IshasRevive = false;
	bool IsinRevive = false;
	bool IsshieldActive = false;
	bool IsinExtraSpeed = false;
	bool Ispaused = false;
	bool IsshopReviveUsed = false;
	bool IspowerUpUsed = false;

	Transform thisTransform;

	public static PlayerManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(PlayerManager)) as PlayerManager;
			}

			return _instance;
		}
	}

	void Start()
	{
		instances++;
		WarningPlayerManager();

		thisTransform = this.GetComponent<Transform>();
		rotationFactor = maxVerticalSpeed / maxRotation;

		characterAnimator = horizontalAnimation.GetComponent<Animator>();
	}

	void WarningPlayerManager()
    {
		if (instances > 1)
        {
			Debug.LogWarning("Warning: There are more than one PlayerManager at the level");
			return;
		}
		_instance = this;
	}

	void Update()
	{
		if (IscontrolEnabled)
		{
			CalculateDistances();
			CalculateMovement();
			MoveAndRotate();
			return;
		}
		if (Iscrashing)
		{
			Crash();
			return;
		}
		speed = 0;
	}

	void CalculateDistances()
	{
		distanceToBottom = thisTransform.position.y - bottomEdge;
		distanceToTop = topEdge - thisTransform.position.y;
	}

	void CalculateMovement()
	{
		MovingSpeed();
	}

	void MovingSpeed()
    {
		if (IsmovingUpward)
        {
            speed += Time.deltaTime * maxVerticalSpeed;
            Isdistance();
			return;
        }
		speed -= Time.deltaTime * maxVerticalSpeed;
		Isdistance();
		return;
	}

    private void Isdistance()
    {
        if (distanceToTop < safeEdgeZone)
        {
            DistanceToTop();
			return;
        }
        if (distanceToBottom < safeEdgeZone)
        {
            DistanceToBottom();
			return;
        }
    }

    void DistanceToBottom()
    {
		newSpeed = maxVerticalSpeed * (bottomEdge - thisTransform.position.y) / safeEdgeZone;

		if (newSpeed > speed)
			speed = newSpeed;
	}

	void DistanceToTop()
    {
		newSpeed = maxVerticalSpeed * (topEdge - thisTransform.position.y) / safeEdgeZone;

		if (newSpeed < speed)
			speed = newSpeed;
	}

	void MoveAndRotate()
	{
		newRotation.z = speed / rotationFactor;

		thisTransform.eulerAngles = newRotation;
		thisTransform.position += Vector3.up * speed * Time.deltaTime;
	}

	void OnTriggerEnter(Collider other)
	{
		IsTriggerCollider(other);
	}

	void IsTriggerCollider(Collider other)
    {
		if (other.tag == "Obstacles")
		{
			IsObstacles(other);
			return;
		}
		if (other.tag == "PowerUps")
		{
			IsPowerUp(other);
			return;
		}
	}

	void IsObstacles(Collider other)
    {
		if (other.transform.name == "Coin")
		{
			IsCoins(other);
			return;
		}
		if (other.transform.name == "BirdBrown" || other.transform.name == "BirdWhite" || other.transform.name == "StorkTall" || other.transform.name == "BirdBody")
		{
			Isbrid(other);
			return;
		}
		if (other.name == "ObstacleSpawnTriggerer" && !IsfirstObstacleSpawned)
		{
			ObstacleSpawnTriggerer();
			return;
		}
	}

	void IsCoins(Collider other)
    {
		other.GetComponent<Renderer>().enabled = false;
		other.GetComponent<Collider>().enabled = false;

		other.transform.Find("CoinParticle").gameObject.GetComponent<ParticleSystem>().Play();
		LevelManager.Instance.CoinGathered();
	}

	void Isbrid(Collider other)
    {
		UpdateMission(other.transform.name);
		if (!Iscrashing && IsCrash && !IsshieldActive)
		{
			Iscrashing = true;
			IscontrolEnabled = false;
			Iscrashed = true;
			return;
		}
		if (IsshieldActive && !IsinExtraSpeed)
		{
			StartCoroutine(DisableShield());
			return;
		}

		PlayHideParticleEffect(other.transform);
	}

	void ObstacleSpawnTriggerer()
    {
		IsfirstObstacleSpawned = true;
		LevelSpawnManager.Instance.SpawnObstacles();
	}

	void IsPowerUp(Collider other)
    {
		UpdateMission(other.transform.name);

		switch (other.transform.name)
		{
			case "ExtraSpeed":
				ExtraSpeed();
				break;

			case "Shield":
				RaiseShield();
				break;

			case "Revive":
				if (IscontrolEnabled)
					ReviveCollected();
				break;

			case "Abracadabra":
				if (IscontrolEnabled)
					StartCoroutine("LaunchAbracadabra");
				break;
		}

		other.GetComponent<PowerUp>().ResetThis();
	}

	void UpdateMission(string name)
	{
		MissionManager.Instance.ObstacleEvent(name);
	}

	void PlayHideParticleEffect(Transform particleParent)
	{
		if (particleParent.name == "BirdBody")
		{
			particleParent.transform.parent.gameObject.GetComponent<BirdTraffic>().TargetHit(true);
			return;
		}
		ParticleSystem hideParticle = particleParent.Find("HideParticleEffect").gameObject.GetComponent("ParticleSystem") as ParticleSystem;
		hideParticle.Play();

		particleParent.GetComponent<Renderer>().enabled = false;
		particleParent.GetComponent<Collider>().enabled = false;
	}

	void Crash()
	{
		Iscrashed = true;

		float crashPosY = bottomEdge - 5;
		float crashPosEdge = 4;

		float distance = thisTransform.position.y - crashPosY;

		if (distanceToTop < safeEdgeZone)
		{
			NewSpeed(topEdge, safeEdgeZone);
		}
		if (distance > 0.1f)
        {
            speed -= Time.deltaTime * maxVerticalSpeed * 0.6f;

            CrashEffect(crashPosY, crashPosEdge, distance);
        }
        else
		{
			Iscrashing = false;
			StartCoroutine("CrashEffects");
		}
	}

	void NewSpeed(float crashPosY, float crashPosEdge)
	{
		newSpeed = maxVerticalSpeed * (crashPosY - thisTransform.position.y) / crashPosEdge;

		if (newSpeed > speed)
			speed = newSpeed;
	}

	private void CrashEffect(float crashPosY, float crashPosEdge, float distance)
    {
        if (distance < crashPosEdge)
        {
            NewSpeed(crashPosY, crashPosEdge);
        }

        MoveAndRotate();

        if (distance < 2.5f)
        {
            ParticleSystem.EmissionModule smokeEmissionModule = smoke.emission;
            smokeEmissionModule.enabled = true;
        }
    }

	IEnumerator CrashEffects()
	{
		yield return new WaitForSeconds(0.5f);
		LevelSpawnManager.Instance.StartCoroutine("StopScrolling", 2.5f);

		yield return new WaitForSeconds(2.75f);

		ParticleSystem.EmissionModule smokeEmissionModule = smoke.emission;
		smokeEmissionModule.enabled = false;
	}

	void ReviveCollected()
	{
		IshasRevive = true;
		GameMenuManager.Instance.RevivePicked();
		LevelSpawnManager.Instance.powerUpManager.DisableReviveGeneration();
	}

	public IEnumerator LaunchAbracadabra()
	{
		abracadabra.SetActive(true);
		IspowerUpUsed = true;

		StartCoroutine(MoveToPosition(abracadabra.transform, new Vector3(GameTransformManager.Instance.AbracadabraPosition, 0, -5), 1.25f, false));

		if (!Ispaused)
		{
			yield return new WaitForSeconds(2.0f);
		}

		abracadabra.GetComponent<AbracadabraEffect>().Disable();
	}

	IEnumerator ScaleObject(Transform obj, Vector3 scale, float time, bool deactivate)
	{
		obj.gameObject.SetActive(true);
		Vector3 startScale = obj.localScale;

		float rate = 1.0f / time;
		float t = 0.0f;
		while (t < 1.0f)
		{
			if (!Ispaused)
			{
				t += Time.deltaTime * rate;
				obj.localScale = Vector3.Lerp(startScale, scale, t);
			}

			yield return new WaitForEndOfFrame();
		}

		if (deactivate)
			obj.gameObject.SetActive(false);

		if (obj.name == "Shield")
			shieldCollider.enabled = true;
	}

	IEnumerator DisableShield()
	{
		shieldCollider.enabled = false;

		if (!Ispaused)
		{
			yield return new WaitForSeconds(0.5f);
		}

		IsshieldActive = false;

		characterAnimator.Play("player");
	}

	IEnumerator MoveToPosition(Transform obj, Vector3 endPos, float time, bool enableControls)
	{
		float i = 0.0f;
		float rate = 1.0f / time;

		Vector3 startPos = obj.position;

		while (i < 1.0)
		{
			if (!Ispaused)
			{
				i += Time.deltaTime * rate;
				obj.position = Vector3.Lerp(startPos, endPos, i);
			}

			yield return new WaitForEndOfFrame();
		}

		if (enableControls)
		{
			IscontrolEnabled = true;
		}
	}

	public void ExtraSpeed()
	{
		if (IsinExtraSpeed || Iscrashing || !IscontrolEnabled)
			return;

		IspowerUpUsed = true;
		IsinExtraSpeed = true;
		IsCrash = false;

		speedParticle.SetActive(true);
		speedTrail.SetActive(true);

		LevelSpawnManager.Instance.ExtraSpeedEffect();
		StartCoroutine(ExtraSpeedEffect(3));
	}

	IEnumerator ExtraSpeedEffect(float time)
	{
		float newSpeed = LevelSpawnManager.Instance.scrollSpeed;
		LevelSpawnManager.Instance.scrollSpeed = 3;

		if (!Ispaused)
		{
			yield return new WaitForSeconds(time);
		}

		LevelSpawnManager.Instance.scrollSpeed = newSpeed;
		IsinExtraSpeed = false;
		IsCrash = true;

		speedParticle.SetActive(false);
		speedTrail.SetActive(false);

		LevelSpawnManager.Instance.ExtraSpeedOver();
	}

	public void RaiseShield()
	{
		if (IsshieldActive || Iscrashing || !IscontrolEnabled)
			return;

		IspowerUpUsed = true;
		IsshieldActive = true;
		characterAnimator.Play("shield");
	}

	public void MoveUp()
	{
		if (distanceToTop > 0 && IscontrolEnabled)
			IsmovingUpward = true;
	}

	public void MoveDown()
	{
		if (distanceToBottom > 0 && IscontrolEnabled)
			IsmovingUpward = false;
	}

	public void DisableControls()
	{
		IscontrolEnabled = false;
	}

	public void EnableControls()
	{
		IscontrolEnabled = true;
	}

	public void ResetStatus(bool moveToStart)
	{
		StopAllCoroutines();

		speed = 0;
		Iscrashed = false;
		Ispaused = false;
		IsmovingUpward = false;
		IsCrash = true;

		IsinRevive = false;
		IshasRevive = false;
		IsshopReviveUsed = false;
		IsinExtraSpeed = false;
		IsshieldActive = false;
		IspowerUpUsed = false;

		speedParticle.SetActive(false);
		speedTrail.SetActive(false);
		IsfirstObstacleSpawned = false;

		newRotation = new Vector3(0, 0, 0);

		this.transform.position = new Vector3(startingPos, 9.0f, thisTransform.position.z);
		this.transform.eulerAngles = newRotation;

		if (moveToStart)
		{
			StartCoroutine(MoveToPosition(this.transform, new Vector3(xPos, 9, thisTransform.position.z), 1.0f, true));
		}
	}

	public void Pause()
	{
		Ispaused = true;
	}

	public void Resume()
	{
		Ispaused = false;
	}

	public bool IsCrashed()
	{
		return Iscrashed;
	}

	public bool HasRevive()
	{
		if (IshasRevive || (PreferencesManager.Instance.GetRevive() > 0 && !IsshopReviveUsed))
			return true;
		else
			return false;
	}

	public bool PowerUpUsed()
	{
		return IspowerUpUsed;
	}

	public void SetPositions(float starting, float main)
	{
		startingPos = starting;
		xPos = main;
	}

	public IEnumerator Revive()
	{
		if (!IsinRevive)
		{
			IsinRevive = true;
			IspowerUpUsed = true;

			IsRevive();

			speed = 0;
			reviveParticle.Play();

			newRotation = new Vector3(0, 0, 0);
			this.transform.eulerAngles = newRotation;

			StartCoroutine("LaunchAbracadabra");

			yield return new WaitForSeconds(0.4f);
			StartCoroutine(MoveToPosition(this.transform, new Vector3(xPos, 9, thisTransform.position.z), 1.0f, false));

			yield return new WaitForSeconds(1.2f);
			LevelSpawnManager.Instance.ContinueScrolling();

			Iscrashed = false;
			IsCrash = true;
			IscontrolEnabled = true;
			IsmovingUpward = false;
			IsinRevive = false;
		}

		yield return new WaitForEndOfFrame();
	}

	void IsRevive()
    {
		if (IshasRevive)
		{
			IshasRevive = false;
			GameMenuManager.Instance.DisableReviveGUI(0);
			return;
		}
		IsshopReviveUsed = true;
		PreferencesManager.Instance.ModifyReviveBy(-1);
		GameMenuManager.Instance.DisableReviveGUI(1);
	}
}