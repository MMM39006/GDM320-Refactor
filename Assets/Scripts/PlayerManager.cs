using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System;

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

	public TimeSpan delayTimeSpan;

	Transform thisTransform;

	public TextMesh bestDist;


	public void AddDelay(TimeSpan delay)
    {
		delayTimeSpan = delay;
    }

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
		AddDelay(new TimeSpan(0, 0, 5));

		if (IscontrolEnabled)
		{
			CalculateDistances();
			CalculateMovement();
			MoveAndRotate();
			return;
		}
		if (Iscrashing)
		{
			AddDelay(new TimeSpan(0,0,5));
			if (delayTimeSpan.TotalSeconds > 0)
			{
				bestDist.text = " (Please Wait for " +delayTimeSpan.Hours.ToString() + " Hours)";
			}
            else
            {
				Crash();
				return;
			}

			
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
		if (IsObStacleIsCoins(other))
		{
			IsCoins(other);
			return;
		}
		if (IsObstacleIsBird(other))
		{
			Isbrid(other);
			return;
		}
		if (IsObstacleIsSpawnTrigger(other))
		{
			ObstacleSpawnTriggerer();
			return;
		}
	}

	bool IsObStacleIsCoins(Collider other)
    {
		return other.transform.name == "Coin";
	}

	bool IsObstacleIsBird(Collider other)
    {
		return other.transform.name == "BirdBrown" || other.transform.name == "BirdWhite" || other.transform.name == "StorkTall" || other.transform.name == "BirdBody";
	}

	bool IsObstacleIsSpawnTrigger(Collider other)
    {
		return other.name == "ObstacleSpawnTriggerer" && !IsfirstObstacleSpawned;
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
			transform.DORotate(new Vector3(0,0,180),1, RotateMode.FastBeyond360);
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
			var CrashEffectData = new PlayerCrashEffectData(crashPosY, crashPosEdge, distance);

			CrashEffect(CrashEffectData);
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

	private void CrashEffect(PlayerCrashEffectData Data)
    {
        if (Data.distance < Data.crashPosEdge)
        {
            NewSpeed(Data.crashPosY, Data.crashPosEdge);
        }

        MoveAndRotate();

        if (Data.distance < 2.5f)
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
		var MoveToPositionData = new PlayerMoveToPositionData(abracadabra.transform, new Vector3(GameTransformManager.Instance.AbracadabraPosition, 0, -5), 1.25f, false);
		StartCoroutine(MoveToPosition(MoveToPositionData));

		if (!Ispaused)
		{
			yield return new WaitForSeconds(2.0f);
		}

		abracadabra.GetComponent<AbracadabraEffect>().Disable();
	}

	IEnumerator ScaleObject(PlayerScaleObjectData Data)
	{
		Data.obj.gameObject.SetActive(true);
		Vector3 startScale = Data.obj.localScale;

		float rate = 1.0f / Data.time;
		float t = 0.0f;
		while (t < 1.0f)
		{
			if (!Ispaused)
			{
				t += Time.deltaTime * rate;
				Data.obj.localScale = Vector3.Lerp(startScale, Data.scale, t);
			}

			yield return new WaitForEndOfFrame();
		}

		if (Data.deactivate)
			Data.obj.gameObject.SetActive(false);

		if (Data.obj.name == "Shield")
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

	IEnumerator MoveToPosition(PlayerMoveToPositionData Data)
	{
		float i = 0.0f;
		float rate = 1.0f / Data.time;

		Vector3 startPos = Data.obj.position;

		while (i < 1.0)
		{
			if (!Ispaused)
			{
				i += Time.deltaTime * rate;
				Data.obj.position = Vector3.Lerp(startPos, Data.endPos, i);
			}

			yield return new WaitForEndOfFrame();
		}

		if (Data.enableControls)
		{
			IscontrolEnabled = true;
		}
	}

	public void ExtraSpeed()
	{
		if (IsShouldNotExtraSpeed())
			return;

		IspowerUpUsed = true;
		IsinExtraSpeed = true;
		IsCrash = false;

		speedParticle.SetActive(true);
		speedTrail.SetActive(true);
		transform.DOScale(new Vector3(1.5f, 1.5f), 1);

		LevelSpawnManager.Instance.ExtraSpeedEffect();
		StartCoroutine(ExtraSpeedEffect(3));
	}

	bool IsShouldNotExtraSpeed()
    {
		return IsinExtraSpeed || Iscrashing || !IscontrolEnabled;
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

		transform.DOScale(new Vector3(1f, 1f), 1);

		speedParticle.SetActive(false);
		speedTrail.SetActive(false);

		LevelSpawnManager.Instance.ExtraSpeedOver();
	}

	public void RaiseShield()
	{
		if (IsShouldNotIsshieActivate())
			return;

		IspowerUpUsed = true;
		IsshieldActive = true;
		characterAnimator.Play("shield");
	}
	bool IsShouldNotIsshieActivate()
    {
		return IsshieldActive || Iscrashing || !IscontrolEnabled;

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
			var MoveToPositionData = new PlayerMoveToPositionData(this.transform, new Vector3(xPos, 9, thisTransform.position.z), 1.0f, true);
			StartCoroutine(MoveToPosition(MoveToPositionData));
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
			var MoveToPositionData = new PlayerMoveToPositionData(this.transform, new Vector3(xPos, 9, thisTransform.position.z), 1.0f, false);
			StartCoroutine(MoveToPosition(MoveToPositionData));

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