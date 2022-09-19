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
        {IsMouseDown();}
        else if (Input.GetMouseButtonUp(0))
        {IsMouseUp();}
    }
    private void IsMouseUp()
    {
        if (button == null)
            PlayerManager.Instance.MoveDown();
        else
            GameMenuManager.Instance.ButtonUp(button);
    }
    private void IsMouseDown()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        IsMouseDownRaycast();
    }
    private void IsMouseDownRaycast()
    {
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {MouseDownBottonHit();}
        else
        {MouseDownButtonNull();}
    }
    private void MouseDownButtonNull()
    {
        button = null;
        PlayerManager.Instance.MoveUp();
    }
    private void MouseDownBottonHit()
    {
        button = hit.transform;
        GameMenuManager.Instance.ButtonDown(button);
    }
    void GetTouches()
	{
		foreach (Touch touch in Input.touches)
        { IsTouch(touch);}
    }
    private void IsTouch(Touch touch)
    {
        if (touch.phase == TouchPhase.Began && touch.phase != TouchPhase.Canceled)
        {TouchCamera(touch);}
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {IsBottonTouch();}
    }
    private void IsBottonTouch()
    {
        if (button == null)
            PlayerManager.Instance.MoveDown();
        else
            GameMenuManager.Instance.ButtonUp(button);
    }
    private void TouchCamera(Touch touch)
    {
        ray = Camera.main.ScreenPointToRay(touch.position);
        IsTouchRaycast();
    }
    private void IsTouchRaycast()
    {
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {TouchBottonHit();}
        else
        {TouchBottonNull();}
    }
    private void TouchBottonNull()
    {
        button = null;
        PlayerManager.Instance.MoveUp();
    }
    private void TouchBottonHit()
    {
        button = hit.transform;
        GameMenuManager.Instance.ButtonDown(button);
    }
}
