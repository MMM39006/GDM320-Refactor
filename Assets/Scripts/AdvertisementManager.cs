using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AdvertisementManager : MonoBehaviour
{
	static AdvertisementManager _instance;
	static int instances = 0;
	public bool showTestAds = true;
	public static AdvertisementManager Instance
	{
		get
        {return getmedInstance();}
    }
    private static AdvertisementManager getmedInstance()
    {
        if (_instance == null)
        {_instance = FindObjectOfType(typeof(AdvertisementManager)) as AdvertisementManager;}
		return _instance;
    }
    void Start()
	{
		instances++;
		if (instances > 1)
			Debug.LogWarning("There are more than one AdvertisementManager");
		else
			_instance = this;
	}
	public void StartAds()
    {
        string appId = functionStartMed();
		Debug.Log("appId: " + appId);
    }
    private static string functionStartMed()
    {
#if UNITY_ANDROID
		string appId = "ca-app-pub-25";
#elif UNITY_IPHONE
		string appId = "ca-app-pub-23";
#else
        return "unexpected_platform";
#endif
    }
    private void InitInterstitial()
    {
#if UNITY_ANDROID
		string adUnitIdReal = "ca-app-pub-23";
#elif UNITY_IPHONE
		string adUnitIdReal = "ca-app-pub-23";
#else
        string adUnitIdReal = "unexpected_platform";
#endif
#if UNITY_ANDROID
		string adUnitIdTest = "ca-app-pub-39";
#elif UNITY_IPHONE
		string adUnitIdTest = "ca-app-pub-39";
#else
        string adUnitIdTest = "unexpected_platform";
#endif
        InitInterMed(adUnitIdReal, adUnitIdTest);
    }
    private void InitInterMed(string adUnitIdReal, string adUnitIdTest)
    {
        string adUnitId = adUnitIdTest;
        if (!this.showTestAds)
            adUnitId = adUnitIdReal;
        Debug.Log("adUnitId: " + adUnitId);
    }
    public void ShowAds()
	{
#if (UNITY_EDITOR)
		GameMenuManager.Instance.RestartGame();
#endif
	}
	public void HandleOnAdClosed(object sender, EventArgs args)
	{
		Debug.Log("HandleAdClosed event received");
		GameMenuManager.Instance.RestartGame();
	}
	private void InitRewardedVideo()
    {
#if UNITY_ANDROID
		string adUnitIdReal = "ca-app-pub-23";
#elif UNITY_IPHONE
		string adUnitIdReal = "ca-app-pub-23";
#else
        string adUnitIdReal = "unexpected_platform";
#endif
#if UNITY_ANDROID
		string adUnitIdTest = "ca-app-pub-39";
#elif UNITY_IPHONE
		string adUnitIdTest = "ca-app-pub-39";
#else
        string adUnitIdTest = "unexpected_platform";
#endif
        RewardVDOMed(adUnitIdReal, adUnitIdTest);
    }

    private void RewardVDOMed(string adUnitIdReal, string adUnitIdTest)
    {
        string adUnitId = adUnitIdTest;
        if (!this.showTestAds)
            adUnitId = adUnitIdReal;
        Debug.Log("adUnitId: " + adUnitId);
    }
    public void WatchAds()
	{}

	public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
	{
		Debug.Log("HandleRewardBasedVideoClosed event received");
		StartCoroutine(GameMenuManager.Instance.UpdateShopRoutine());
	}
}