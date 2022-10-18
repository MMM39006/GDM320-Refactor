using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class GameMenuManager : MonoBehaviour
{


	public Renderer overlay;

	[Header("Scalable Element Settings")]
	public GameObject[] headerLefts;
	public GameObject[] headerRights;
	public GameObject[] mainGUILefts;
	public GameObject[] mainGUIRights;
	public GameObject[] scalables;
	public GameObject[] backButtons;

	public TextMesh bestDist;
	public Texture2D[] menuTextures;
	public GameObject[] mainMenuElements;

	[Header("Shop Menu Settings")]
	public TextMesh[] shopTexts;
	public int[] shopPrices;


	public GameObject[] shopElements;

	[Header("Main GUI Settings")]
	public GameObject[] startPowerUps;
	public TextMesh[] guiTexts;
	public GameObject[] mainGUIElements;

	[Header("Pause Menu Settings")]
	public GameObject[] pauseElements;
	[Header("Finish Menu Settings")]
	public GameObject finishMenu;
	public GameObject finishMenuTop;
	public GameObject finishMenuTopHeader;
	public TextMesh[] finishTexts;

	[Header("Mission Menu Settings")]
	public GameObject[] missionNotification;
	public TextMesh[] missionTexts;
	public TextMesh[] missionStatus;

	static GameMenuManager _instance;
	static int instances = 0;

	bool IsshowMainGUI = false;
	public bool IsshowPause = false;
	bool IsmainMenuTopHidden = true;
	public bool IsaudioEnabled = true;
	bool IsmainMissionHidden = true;
	bool IsshopHidden = true;
	bool IsaboutHidden = true;
	bool IsareYouSureHidden = true;

	bool Isstarting = false;
	bool IsreviveActivated = false;

	bool IscanClick = true;

	bool IsmNotification1Used = false;
	bool IsmNotification2Used = false;
	bool IsmNotification3Used = false;

	public int restartCount = 0;

	[SerializeField] ShopPriceDataBase shopPriceDatabase;


	public static GameMenuManager Instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType(typeof(GameMenuManager)) as GameMenuManager;

			return _instance;
		}
	}

	void Start()
	{
		instances++;
		WarningGameMenuManager();
	}

	void WarningGameMenuManager()
    {
		if (instances > 1)
        {
			Debug.LogWarning("Warning: There are more than one GameMenuManager at the level");
			return;
		}
		_instance = this;
	}

	void Update()
	{
		ShowMainGui();
	}

	void ShowMainGui()
    {
		if (IsshowMainGUI && !IsshowPause)
		{
			var SpawnDataInstanceCoins = new SpawnData (guiTexts[0], LevelManager.Instance.Coins(), 4);
			var SpawnDataInstanceDistance = new SpawnData(guiTexts[1], (int)LevelSpawnManager.Instance.distance, 5);
			DisplayStatistics(SpawnDataInstanceCoins);
			DisplayStatistics(SpawnDataInstanceDistance);
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ToggleAreYouSure();
		}
	}

	void DisplayStatistics(SpawnData Data)
	{
		string dataString = "";
		int remainingDigitCount = Data.digitNumbers - Data.data.ToString().Length;

		RemainDgigigCount(remainingDigitCount, dataString);

		dataString += Data.data;
		Data.target.text = dataString;
	}

	void RemainDgigigCount(int remainingDigitCount,string dataString)
    {
		while (remainingDigitCount > 0)
		{
			dataString += "0";
			remainingDigitCount--;
		}
	}

	void Pause()
	{
		if (PlayerManager.Instance.IsCrashed())
			return;

		PlayerManager.Instance.Pause();
		LevelManager.Instance.PauseGame();
		IsshowPause = true;

		pauseElements[0].SetActive(true);

		StartCoroutine(FadeScreen(0.4f, 0.7f));
		StartCoroutine(MoveMenu(pauseElements[1].transform, 0, 29, 0.45f, false));
		StartCoroutine(MoveMenu(pauseElements[2].transform, 0, 0, 0.45f, false));

		SoundManager.Instance.PauseMusic();
	}

	void ToggleMainMenuArrowButtonDown(Material arrow)
	{
		if (IsmainMenuTopHidden)
		{
			IsmainMenuTopHidden = false;
			arrow.mainTexture = menuTextures[1];
			StartCoroutine(FadeScreen(0.25f, 0.7f));
			StartCoroutine(MoveMenu(mainMenuElements[1].transform, 0, 22, 0.25f, false));
			return;
		}
		if (!IsmainMissionHidden)
		{
			IsmainMissionHidden = true;
			StartCoroutine(MoveMenu(mainMenuElements[3].transform, 0, -31, 0.4f, false));
			return;
		}

		IsmainMenuTopHidden = true;
		arrow.mainTexture = menuTextures[0];
		StartCoroutine(FadeScreen(0.25f, 0));
		StartCoroutine(MoveMenu(mainMenuElements[1].transform, 0, 32, 0.25f, false));
	}

	public void ToggleAudio(Material audioButton)
	{
		if (!IsshopHidden)
			return;

		if (IsaudioEnabled)
		{
			SoundManager.Instance.SetMusicVolume(0.0f);
			IsaudioEnabled = false;
			audioButton.mainTexture = menuTextures[3];
		}
		else
		{
			SoundManager.Instance.SetMusicVolume(1.0f);
			audioButton.mainTexture = menuTextures[2];
			IsaudioEnabled = true;
		}

		SoundManager.Instance.CheckVolumeButtons();
	}

	void ToggleMainMissionList()
	{
		if (!IsshopHidden)
			return;

		if (IsmainMissionHidden)
		{
			IsmainMissionHidden = false;
			StartCoroutine(MoveMenu(mainMenuElements[3].transform, 0, -75, 0.4f, false));
			return;
		}
		IsmainMissionHidden = true;
		StartCoroutine(MoveMenu(mainMenuElements[3].transform, 0, -31, 0.4f, false));
	}

	void ToggleShopMenu()
    {
        HiddenMenu(IsshopHidden,2);
    }

    private void HiddenMenu(bool MenuToshow, int Elements)
    {
        if (MenuToshow)
        {
            UpdateShop();
            SoundManager.Instance.PauseMusic();
			MenuToshow = false;
            StartCoroutine(MoveMenu(mainMenuElements[Elements].transform, 0, -21.85f, 0.45f, false));
        }
        else
        {
			MenuToshow = true;
            StartCoroutine(MoveMenu(mainMenuElements[Elements].transform, 0, -94, 0.45f, false));
        }
        mainMenuElements[1].SetActive(MenuToshow);
    }

    void ToggleAboutUs()
	{
		HiddenMenu(IsaboutHidden, 8);
	}

	void ToggleAreYouSure()
	{
		HiddenMenu(IsareYouSureHidden, 9);
	}

	public void UpdateShop()
	{
		var shopPrices = shopPriceDatabase.GetItemPriceData();
		//coin
		shopTexts[0].text = PreferencesManager.Instance.GetCoins().ToString();
		//extra speed
		shopTexts[1].text = "x " + PreferencesManager.Instance.GetExtraSpeed();
		shopTexts[2].text = shopPrices.ToString();
		//shield
		shopTexts[3].text = "x " + PreferencesManager.Instance.GetShield();
		shopTexts[4].text = shopPrices.ToString();
		//abracadabra
		shopTexts[5].text = "x " + PreferencesManager.Instance.GetAbracadabra();
		shopTexts[6].text = shopPrices.ToString();
		//revive
		shopTexts[7].text = "x " + PreferencesManager.Instance.GetRevive();
		shopTexts[8].text = shopPrices.ToString();
	}

	public IEnumerator UpdateShopRoutine()
	{
		yield return new WaitForSeconds(2);
		UpdateShop();
	}

	public void GetRewardCoin(double amount)
	{
		PreferencesManager.Instance.AddCoins(System.Convert.ToInt32(amount));
	}

	void IsBuySpeedPowerup()
	{
		//var shopPrices = shopPriceDatabase.GetItemPriceData();
		if (PreferencesManager.Instance.GetCoins() >= shopPrices[0])
		{
			PreferencesManager.Instance.ReduceCoins(shopPrices[0]);
			PreferencesManager.Instance.ModifyExtraSpeedBy(1);
			FirebaseEventManager.Instance.SendSpendVirtualCurrency();

			UpdateShop();
			MissionManager.Instance.ShopEvent("ExtraSpeed");
		}
	}

	void IsBuyShieldPowerup()
	{
		if (PreferencesManager.Instance.GetCoins() >= shopPrices[1])
		{
			PreferencesManager.Instance.ReduceCoins(shopPrices[0]);
			PreferencesManager.Instance.ModifyShieldBy(1);
			FirebaseEventManager.Instance.SendSpendVirtualCurrency();

			UpdateShop();
			MissionManager.Instance.ShopEvent("Shield");
		}
	}

	void IsBuyAbracadabraPowerup()
	{
		if (PreferencesManager.Instance.GetCoins() >= shopPrices[2])
		{
			PreferencesManager.Instance.ReduceCoins(shopPrices[0]);
			PreferencesManager.Instance.ModifyAbracadabraBy(1);
			FirebaseEventManager.Instance.SendSpendVirtualCurrency();

			UpdateShop();
			MissionManager.Instance.ShopEvent("Abracadabra");
		}
	}

	void IsBuyRevivePowerup()
	{
		if (PreferencesManager.Instance.GetCoins() >= shopPrices[3])
		{
			PreferencesManager.Instance.ReduceCoins(shopPrices[0]);
			PreferencesManager.Instance.ModifyReviveBy(1);
			FirebaseEventManager.Instance.SendSpendVirtualCurrency();

			UpdateShop();
			MissionManager.Instance.ShopEvent("Revive");
		}
	}

	//Activate the extra speed powerup at startup
	void ActivateSpeedPowerup()
	{
		if (IsshowPause)
			return;

		StartCoroutine(MoveMenu(startPowerUps[0].transform, startPowerUps[0].transform.localPosition.x, -45, 0.4f, true));

		PlayerManager.Instance.ExtraSpeed();
		PreferencesManager.Instance.ModifyExtraSpeedBy(-1);

		StopCoroutine("MovePowerUpSelection");
	}

	//Activate the shield powerup at startup
	void ActivateShieldPowerup()
	{
		if (IsshowPause)
			return;

		StartCoroutine(MoveMenu(startPowerUps[1].transform, startPowerUps[1].transform.localPosition.x, -45, 0.4f, true));

		PlayerManager.Instance.RaiseShield();
		PreferencesManager.Instance.ModifyShieldBy(-1);

		StopCoroutine("MovePowerUpSelection");
	}

	//Activate the abracadabra powerup at startup
	void ActivateAbracadabraPowerup()
	{
		if (IsshowPause)
			return;

		StartCoroutine(MoveMenu(startPowerUps[2].transform, startPowerUps[2].transform.localPosition.x, -45, 0.4f, true));

		StartCoroutine(PlayerManager.Instance.LaunchAbracadabra());
		PreferencesManager.Instance.ModifyAbracadabraBy(-1);

		StopCoroutine("MovePowerUpSelection");
	}


	//Activate the revive powerup at crash
	void ActivateRevivePowerup()
	{
		IsreviveActivated = true;
	}

	void StartToPlay()
	{
		mainMenuElements[10].SetActive(false);

		if (IsShouldNotStartToPlay())
			return;

		Isstarting = true;

		if (PreferencesManager.Instance.GetRevive() > 0)
			mainGUIElements[2].SetActive(true);

		if (!IsmainMenuTopHidden)
		{
			StartCoroutine(FadeScreen(0.25f, 0));
			StartCoroutine(MoveMenu(mainMenuElements[1].transform, 0, 32, 0.25f, true));
			return;
		}
		mainMenuElements[1].SetActive(false);


		PlayerManager.Instance.startAnimation.GetComponent<Animator>().enabled = true;
		SoundManager.Instance.StartMusic();
	}

	bool IsShouldNotStartToPlay()
    {
		return !IsmainMissionHidden || !IsshopHidden || Isstarting || !IsaboutHidden || !IsareYouSureHidden;
	}

	IEnumerator SpawnObstacles()
	{
		yield return new WaitForSeconds(5);
		PlayerManager.Instance.IsfirstObstacleSpawned = true;
		LevelSpawnManager.Instance.SpawnObstacles();
	}

	IEnumerator Resume()
	{
		StartCoroutine(FadeScreen(0.4f, 0));
		StartCoroutine(MoveMenu(pauseElements[1].transform, 0, 59, 0.45f, false));
		StartCoroutine(MoveMenu(pauseElements[2].transform, 0, -60, 0.45f, false));

		yield return new WaitForSeconds(0.6f);
		IsshowPause = false;

		PlayerManager.Instance.Resume();
		LevelManager.Instance.ResumeGame();
		SoundManager.Instance.ResumeMusic();
	}

	public IEnumerator RestartRoutine()
	{
		StartCoroutine(FadeScreen(0.4f, 1.0f));

		IsShowPause();

		yield return new WaitForSeconds(0.5f);
		StartCoroutine(FadeScreen(0.4f, 0.0f));

		mainMenuElements[0].SetActive(false);
		LevelManager.Instance.Restart();
		SoundManager.Instance.StartMusic();
	}

	void IsShowPause()
    {
		if (IsshowPause)
		{
			IsshowPause = false;
			StartCoroutine(MoveMenu(pauseElements[1].transform, 0, 59, 0.45f, false));
			StartCoroutine(MoveMenu(pauseElements[2].transform, 0, -60, 0.45f, false));
		}
		else
		{
			StartCoroutine(MoveMenu(finishMenu.transform, 0, -60, 0.55f, false));
			StartCoroutine(MoveMenu(finishMenuTop.transform, 0, 118, 0.55f, false));
		}
	}

	public void RestartGame()
	{
		StartCoroutine(RestartRoutine());
	}

	IEnumerator ShowAds()
	{
		AdvertisementManager.Instance.ShowAds();
		yield return new WaitForSeconds(0.01f);
	}

	IEnumerator QuitToMain()
    {
        Isstarting = false;
        StartCoroutine(FadeScreen(0.4f, 1.0f));

        IsShowPause();

        mainGUIElements[1].SetActive(false);
        mainGUIElements[2].SetActive(false);

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeScreen(0.4f, 0.0f));

        mainMenuElements[0].SetActive(true);
        LevelManager.Instance.QuitToMain();

        DisableComponentToMain();
    }

    private static void DisableComponentToMain()
    {
        PlayerManager.Instance.horizontalAnimation.GetComponent<Animator>().enabled = false;
        PlayerManager.Instance.horizontalAnimation.GetComponent<SpriteRenderer>().enabled = false;
        PlayerManager.Instance.startAnimation.GetComponent<Animator>().enabled = false;
        PlayerManager.Instance.startAnimation.GetComponent<SpriteRenderer>().enabled = true;
        PlayerManager.Instance.startAnimation.GetComponent<SpriteRenderer>().sprite = PlayerManager.Instance.startAnimation.GetComponent<ChangeAvatar>().startSprite;

        SoundManager.Instance.StopMusic();
    }

    //---------------------------------------------------------------------------
    public IEnumerator MoveMenu(Transform menuTransform, float endPosX, float endPosY, float time, bool hide)
	{
		IscanClick = false;

		float i = 0.0f;
		float rate = 1.0f / time;

		Vector3 startPos = menuTransform.localPosition;
		Vector3 endPos = new Vector3(endPosX, endPosY, menuTransform.localPosition.z);

		while (i < 1.0)
		{
			i += Time.deltaTime * rate;
			menuTransform.localPosition = Vector3.Lerp(startPos, endPos, i);
			yield return new WaitForEndOfFrame();
		}

		if (hide)
			menuTransform.gameObject.SetActive(false);

		IscanClick = true;
	}

	IEnumerator MovePowerUpSelection(bool speed, bool shield, bool abracadabra)
    {
        yield return new WaitForSeconds(3.0f);
		var Menuposition = new MenuPositionData(speed, shield, abracadabra, -28.5f, false);
        MenuPosition(Menuposition);

        if (!IsshowPause)
        {
            yield return new WaitForSeconds(10.0f);
        }
		Menuposition = new MenuPositionData(speed, shield, abracadabra, -45f, false);
		MenuPosition(Menuposition);


		yield return new WaitForSeconds(0.4f);

        StopCoroutine("MovePowerUpSelection");
    }

    private void MenuPosition(MenuPositionData Data)
    {
        if (Data.speed)
            StartCoroutine(MoveMenu(startPowerUps[0].transform, startPowerUps[0].transform.localPosition.x, Data.positionMove, 0.4f, false));
        if (Data.shield)
            StartCoroutine(MoveMenu(startPowerUps[1].transform, startPowerUps[1].transform.localPosition.x, Data.positionMove, 0.4f, false));
        if (Data.abracadabra)
            StartCoroutine(MoveMenu(startPowerUps[2].transform, startPowerUps[2].transform.localPosition.x, Data.positionMove, 0.4f, false));
    }

    public void SetLevelResolution()
	{
		GameTransformManager.Instance.SetGameTransformByAspectRatio(scalables, shopElements, headerLefts, mainGUILefts, headerRights, mainGUIRights, backButtons, mainMenuElements[0], mainMenuElements[10], LevelSpawnManager.Instance.galata, mainMenuElements[11]);
	}

	public void ButtonDown(Transform button)
	{
		if (!IscanClick)
			return;

		Vector3 scale = button.transform.localScale;
		button.transform.localScale = scale * 0.8f;
	}

	public void ButtonUp(Transform button)
	{
		if (!IscanClick)
			return;

		Vector3 scale = button.transform.localScale;
		button.transform.localScale = scale * 1.25f;

		switch (button.name)
		{
			case "PauseShowButton":
				Pause();
				break;

			case "Resume":
				StartCoroutine(Resume());
				break;

			case "Retry":
				StartCoroutine(ShowAds());
				break;

			case "Home":
				StartCoroutine(QuitToMain());
				break;

			case "YesQuit":
				Application.Quit();
				break;

			case "Quit":
			case "BackAreYouSure":
				ToggleAreYouSure();
				break;

			case "MainArrow":
				ToggleMainMenuArrowButtonDown(button.GetComponent<Renderer>().material);
				break;

			case "AudioEnabler":
				ToggleAudio(button.GetComponent<Renderer>().material);
				break;

			case "Missions":
				ToggleMainMissionList();
				break;

			case "Shop":
			case "Back":
				ToggleShopMenu();
				break;

			case "AboutUs":
			case "BackAbout":
				ToggleAboutUs();
				break;

			case "PlayTriggerer":
				StartToPlay();
				break;

			case "BuySpeed":
				IsBuySpeedPowerup();
				break;

			case "BuyShield":
				IsBuyShieldPowerup();
				break;

			case "BuyRevive":
				IsBuyRevivePowerup();
				break;

			case "BuyAdvertisement":
				AdvertisementManager.Instance.WatchAds();
				break;

			case "BuyAbracadabra":
				IsBuyAbracadabraPowerup();
				break;

			case "ExtraSpeedActivation":
				ActivateSpeedPowerup();
				break;

			case "ShieldActivation":
				ActivateShieldPowerup();
				break;

			case "AbracadabraActivation":
				ActivateAbracadabraPowerup();
				break;

			case "ReviveActivation":
				ActivateRevivePowerup();
				break;
		}
	}

	public void ShowEnd()
    {
        SoundManager.Instance.StopMusic();

        MissionManager.Instance.Save();
        finishMenu.SetActive(true);
        finishMenuTop.SetActive(true);

        int currentDist = (int)LevelSpawnManager.Instance.distance;
        int currentCoins = LevelManager.Instance.Coins();

        finishTexts[0].text = currentDist + "M";
        finishTexts[1].text = currentCoins.ToString();

        if (currentDist > PreferencesManager.Instance.GetBestDistance())
            PreferencesManager.Instance.SetBestDistance(currentDist);

        PreferencesManager.Instance.SetCoins(PreferencesManager.Instance.GetCoins() + currentCoins);
        FirebaseEventManager.Instance.SendEarnVirtualCurrency();

        MoveScreenEndUiGame();

        restartCount++;
    }

    private void MoveScreenEndUiGame()
    {
        StartCoroutine(FadeScreen(0.4f, 0.7f));
        StartCoroutine(MoveMenu(finishMenu.transform, 0, 67, 0.55f, false));
        StartCoroutine(MoveMenu(finishMenuTop.transform, 0, -37, 0.55f, false));
    }

    public void ShowStartPowerUps()
    {
        bool hasSpeed = PreferencesManager.Instance.GetExtraSpeed() > 0;
        bool hasShield = PreferencesManager.Instance.GetShield() > 0;
        bool hasAbracadabra = PreferencesManager.Instance.GetAbracadabra() > 0;

        int numberOfPowerUps = 0;

        if (hasSpeed)
            numberOfPowerUps++;
        if (hasShield)
            numberOfPowerUps++;
        if (hasAbracadabra)
            numberOfPowerUps++;

		var NumberPowerUpData = new GameMenuNumberPowerUpData(hasSpeed, hasShield, hasAbracadabra, numberOfPowerUps);

		NumberPowerUp(NumberPowerUpData);

        StartCoroutine(MovePowerUpSelection(hasSpeed, hasShield, hasAbracadabra));
    }

    private void NumberPowerUp(GameMenuNumberPowerUpData Data)
    {
        if (Data.numberOfPowerUps == 1)
        {
            if (Data.hasSpeed)
            {
                startPowerUps[0].transform.localPosition = new Vector3(0, -40, 0);
                startPowerUps[0].SetActive(true);
				return;
            }
            if (Data.hasShield)
            {
                startPowerUps[1].transform.localPosition = new Vector3(0, -40, 0);
                startPowerUps[1].SetActive(true);
				return;
            }
			startPowerUps[2].transform.localPosition = new Vector3(0, -40, 0);
			startPowerUps[2].SetActive(true);
			return;
        }
        if (Data.numberOfPowerUps == 2)
        {
            if (Data.hasSpeed)
            {
                startPowerUps[0].transform.localPosition = new Vector3(-7.5f, -40, 0);
                startPowerUps[0].SetActive(true);
            }
            if (Data.hasShield)
            {
                if (Data.hasSpeed)
                    startPowerUps[1].transform.localPosition = new Vector3(7.5f, -40, 0);
                else
                    startPowerUps[1].transform.localPosition = new Vector3(-7.5f, -40, 0);

                startPowerUps[1].SetActive(true);
            }
            if (Data.hasAbracadabra)
            {
                startPowerUps[2].transform.localPosition = new Vector3(7.5f, -40, 0);
                startPowerUps[2].SetActive(true);
            }
			return;
        }
        if (Data.numberOfPowerUps == 3)
        {
            startPowerUps[0].transform.localPosition = new Vector3(-15, -40, 0);
            startPowerUps[0].SetActive(true);
            startPowerUps[1].transform.localPosition = new Vector3(0, -40, 0);
            startPowerUps[1].SetActive(true);
            startPowerUps[2].transform.localPosition = new Vector3(15, -40, 0);
            startPowerUps[2].SetActive(true);
			return;
        }
    }

    public void ActivateMainGUI()
	{
		IsshowMainGUI = true;
		mainGUIElements[0].SetActive(true);
	}

	public void DeactivateMainGUI()
	{
		IsshowMainGUI = false;
		mainGUIElements[0].SetActive(false);
	}

	public void ActivateMainMenu()
	{
		mainMenuElements[4].GetComponent<Renderer>().material.mainTexture = menuTextures[0];
		mainMenuElements[1].SetActive(true);
	}

	public void DeactivateMainMenu()
	{
		mainMenuElements[1].SetActive(false);
	}

	public void RevivePicked()
	{
		mainGUIElements[1].SetActive(true);
	}

	public void DisableReviveGUI(int num)
	{
		if (num == 0 || num == 1)
			mainGUIElements[num + 1].SetActive(false);
	}

	public void UpdateBestDistance()
	{
		bestDist.text = PreferencesManager.Instance.GetBestDistance() + "M";
	}

	public void UpdateMissionTexts(string text1, string text2, string text3)
	{
		if (text1.Length < 26)
		{
			MissionFontSize(20, 0, 1, 2);
			return;
		}
		if (text1.Length < 31)
		{
			MissionFontSize(18, 0, 1, 2);
			return;
		}
		MissionFontSize(14, 0, 1, 2);

		MissionText(text1, 0, 1, 2);

		if (text2.Length < 26)
		{
			MissionFontSize(20, 3, 4, 5);
			return;
		}
		if (text2.Length < 31)
		{
			MissionFontSize(18, 3, 4, 5);
			return;
		}
		MissionFontSize(14, 3, 4, 5);
		MissionText(text2, 3, 4, 5);


		if (text3.Length < 26)
		{
			MissionFontSize(20, 6, 7, 8);
			return;
		}
		if (text3.Length < 31)
		{
			MissionFontSize(18, 6, 7, 8);
			return;
		}
		MissionFontSize(14, 6, 7, 8);
		MissionText(text3, 6, 7, 8);
	}

	void MissionFontSize(int Size, int n0, int n1, int n2)
	{
		missionTexts[n0].fontSize = Size;
		missionTexts[n1].fontSize = Size;
		missionTexts[n2].fontSize = Size;
	}

	void MissionText(string text,int n0,int n1,int n2)
    {
		missionTexts[n0].text = text;
		missionTexts[n1].text = text;
		missionTexts[n2].text = text;
	}


	public void UpdateMissionStatus(int i, int a, int b)
	{
		switch (i)
		{
			case 0:
				MissionStatus(a, b, 0, 1, 2);
				break;

			case 1:
				MissionStatus(a, b, 3, 4, 5);
				break;

			case 2:
				MissionStatus(a, b, 6, 7, 8);
				break;
		}
	}

	void MissionStatus(int a ,int b,int n0,int n1,int n2)
    {
		missionStatus[n0].text = a + "/" + b;
		missionStatus[n1].text = a + "/" + b;
		missionStatus[n2].text = a + "/" + b;
	}

	public IEnumerator ShowRevive()
	{
		StartCoroutine(MoveMenu(startPowerUps[3].transform, startPowerUps[3].transform.localPosition.x, -29.5f, 0.4f, false));


		bool activated = false;

		double waited = 0;
		while (waited <= 3)
		{
			waited += Time.deltaTime;

			if (IsreviveActivated)
			{
				yield return new WaitForSeconds(0.2f);
				StartCoroutine(MoveMenu(startPowerUps[3].transform, startPowerUps[3].transform.localPosition.x, -45, 0.4f, false));

				yield return new WaitForSeconds(0.4f);
				LevelManager.Instance.Revive();
				IsreviveActivated = false;
				activated = true;
			}

			yield return new WaitForEndOfFrame();
		}

		if (!activated)
		{
			StartCoroutine(MoveMenu(startPowerUps[3].transform, startPowerUps[3].transform.localPosition.x, -45, 0.4f, false));
			yield return new WaitForSeconds(0.5f);

			ShowEnd();
		}
	}

	public IEnumerator ShowMissionComplete(string text)
	{
		GameObject notificationObject = null;
		TextMesh notificationTextMesh = null;

		int notificationIndex = 0;
		float yPosTarget = 0;

		if (!IsmNotification1Used)
		{
			Notification(notificationObject, notificationTextMesh, 0, 1, 32);
		}
		else if (IsmNotification1Used && !IsmNotification2Used)
		{
			Notification(notificationObject, notificationTextMesh, 1, 2, 26);
		}
		else if (IsmNotification1Used && IsmNotification2Used && !IsmNotification3Used)
		{
			Notification(notificationObject, notificationTextMesh, 2, 3, 20);
		}
		else
		{
			StopCoroutine("ShowMissionComplete");
		}

		if (text.Length < 26)
			notificationTextMesh.fontSize = 24;
		else if (text.Length < 31)
			notificationTextMesh.fontSize = 21;
		else if (text.Length < 36)
			notificationTextMesh.fontSize = 19;
		else
			notificationTextMesh.fontSize = 14;
		//notificationTextsize( text, notificationTextMesh);

		notificationTextMesh.text = text;

		StartCoroutine(MoveMenu(notificationObject.transform, 0, yPosTarget, 0.4f, false));

		if (!IsshowPause)
		{
			yield return new WaitForSeconds(2.0f);
		}

		StartCoroutine(MoveMenu(notificationObject.transform, 0, 38.5f, 0.4f, false));

		if (!IsshowPause)
		{
			yield return new WaitForSeconds(0.5f);
		}

		IndexNotification(notificationIndex);
	}

	void Notification(GameObject notificationObject, TextMesh notificationTextMesh, int missionNotificationIndex, int notificationIndex, float yPosTarget)
    {
		notificationObject = missionNotification[missionNotificationIndex];
		notificationTextMesh = notificationObject.transform.Find("Text").GetComponent<TextMesh>() as TextMesh;

		IsmNotification1Used = true;
		notificationIndex = 1;
		yPosTarget = 32;
	}

	void notificationTextsize(string text, TextMesh notificationTextMesh)
    {
		if (text.Length < 26)
			notificationTextMesh.fontSize = 24;
		else if (text.Length < 31)
			notificationTextMesh.fontSize = 21;
		else if (text.Length < 36)
			notificationTextMesh.fontSize = 19;
		else
			notificationTextMesh.fontSize = 14;
	}

	void IndexNotification(int notificationIndex)
    {
		if (notificationIndex == 1)
			IsmNotification1Used = false;
		else if (notificationIndex == 2)
			IsmNotification2Used = false;
		else if (notificationIndex == 3)
			IsmNotification3Used = false;
	}

	public IEnumerator FadeScreen(float time, float to)
	{
		float i = 0.0f;
		float rate = 1.0f / time;

		Color start = overlay.material.color;
		Color end = new Color(start.r, start.g, start.b, to);

		while (i < 1.0)
		{
			i += Time.deltaTime * rate;
			overlay.material.color = Color.Lerp(start, end, i);
			yield return new WaitForEndOfFrame();
		}
	}
}
