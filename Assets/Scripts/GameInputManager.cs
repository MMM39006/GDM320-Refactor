using UnityEngine;
using System.Collections;
public class GameInputManager : MonoBehaviour
{
	public bool useTouch = false;
	public LayerMask mask = -1;
	Ray ray;
	RaycastHit hit;
	Transform button;
	void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        StartCoroutine(CaptureScreenshot());
        updateMethod();
    }
    private void updateMethod()
    {
        if (useTouch)
            GetTouches();
        else
            GetClicks();
    }
    IEnumerator CaptureScreenshot()
	{
		string filename = GetFileName(Screen.width, Screen.height);
		Debug.LogError("Screenshot saved to " + filename);
		ScreenCapture.CaptureScreenshot(filename);
		yield return new WaitForSeconds(0.1f);
	}
	string GetFileName(int width, int height)
	{
		return string.Format("screenshot_{0}x{1}_{2}.png", width, height, System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
	}
	void GetClicks()
	{
		if (Input.GetMouseButtonDown(0))
        {mouseDownMed();}
        else if (Input.GetMouseButtonUp(0))
        {MouseUpMed();}
    }
    private void MouseUpMed()
    {
        if (button == null)
            PlayerManager.Instance.MoveDown();
        else
            GameMenuManager.Instance.ButtonUp(button);
    }
    private void mouseDownMed()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        mousedownMed2();
    }
    private void mousedownMed2()
    {
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {down2Med();}
        else
        {down2med2();}
    }
    private void down2med2()
    {
        button = null;
        PlayerManager.Instance.MoveUp();
    }
    private void down2Med()
    {
        button = hit.transform;
        GameMenuManager.Instance.ButtonDown(button);
    }
    void GetTouches()
	{
		foreach (Touch touch in Input.touches)
        { touchMed1(touch);}
    }
    private void touchMed1(Touch touch)
    {
        if (touch.phase == TouchPhase.Began && touch.phase != TouchPhase.Canceled)
        {touchmedif1(touch);}
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {touchmedelse1();}
    }
    private void touchmedelse1()
    {
        if (button == null)
            PlayerManager.Instance.MoveDown();
        else
            GameMenuManager.Instance.ButtonUp(button);
    }
    private void touchmedif1(Touch touch)
    {
        ray = Camera.main.ScreenPointToRay(touch.position);
        touchmedif2();
    }
    private void touchmedif2()
    {
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {touchmedif3();}
        else
        {touchmedifelse();}
    }
    private void touchmedifelse()
    {
        button = null;
        PlayerManager.Instance.MoveUp();
    }
    private void touchmedif3()
    {
        button = hit.transform;
        GameMenuManager.Instance.ButtonDown(button);
    }
}
