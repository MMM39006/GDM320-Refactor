using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FirebaseEventManager : MonoBehaviour
{
	static FirebaseEventManager _instance;
	static int instances = 0;
	public bool isReady = false;
	public static FirebaseEventManager Instance
	{
		get
        { return instancefirebasemedthod();}
	}
    private static FirebaseEventManager instancefirebasemedthod()
    {
        if (_instance == null)
        {_instance = FindObjectOfType(typeof(FirebaseEventManager)) as FirebaseEventManager;}
        return _instance;
    }
    void Start()
    {
        instances++;
        instancesMethod();
    }
    private void instancesMethod()
    {
        if (instances > 1)
        { Debug.LogWarning("There are more than one FirebaseEventManager");}
        else
        {_instance = this;}
    }
	public void SendFirstOpenEvent()
	{
		if (isReady)
        {send1openMethod();}
    }
    private static void send1openMethod()
    {
        if (!PreferencesManager.Instance.GetFirtsOpen())
        { PreferencesManager.Instance.SetFirtsOpen(); }
    }
    public void SendSessionStart()
	{
		if (isReady){}
	}
	public void SendEarnVirtualCurrency()
	{
		if (isReady){}
	}
	public void SendSpendVirtualCurrency()
	{
		if (isReady){}
	}
	public void SendLevelUp()
	{
		if (isReady){}
	}
	public void SendCustomEvent(string name)
	{
		if (isReady){}
	}
}
