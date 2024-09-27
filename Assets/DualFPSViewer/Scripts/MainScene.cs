using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : MonoBehaviour
{

	int _screenWidth;
	int _screenHeight;

	const int LogMax = 15;
	readonly List<string> _logs = new();

	void Start()
	{
		UpdateScreenSize();

		int refreshRate = GetRefreshRate();
		Application.targetFrameRate = refreshRate;
		AddLog($"RefreshRate:{refreshRate}");
	}

	void Update()
	{
		if (_screenWidth != Screen.width || _screenHeight != Screen.height) {
			UpdateScreenSize();
		}
	}

	int GetRefreshRate()
	{
		int refreshRate = (int)Screen.currentResolution.refreshRateRatio.value;
#if UNITY_ANDROID && !UNITY_EDITOR
		try {
			using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
				AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaObject windowManager = activity.Call<AndroidJavaObject>("getWindowManager");
				AndroidJavaObject display = windowManager.Call<AndroidJavaObject>("getDefaultDisplay");
				refreshRate = (int)display.Call<float>("getRefreshRate");
			}
		}
		catch (System.Exception e) {
			AddLog("Failed to get refresh rate: " + e.Message);
		}
#endif
		return refreshRate;
	}

	void UpdateScreenSize()
	{
		_screenWidth = Screen.width;
		_screenHeight = Screen.height;
		AddLog($"UpdateScreenSize width:{_screenWidth} height:{_screenHeight}");
	}

	#region Log
	void AddLog(string log, bool isDebugLog = true)
	{
		if (isDebugLog) Debug.Log(log);
		_logs.Add(log);
	}

	void OnGUI()
	{
		float width = _screenWidth;
		float height = 20.0f;
		Rect position = new(0.0f, _screenHeight - (height * LogMax), width, height);

		if (_logs.Count > LogMax) {
			_logs.RemoveRange(0, _logs.Count - LogMax);
		}

		for (int i = _logs.Count - 1; i >= 0; i--) {
			GUI.Label(position, _logs[i]);
			position.y += height;
		}
	}
	#endregion
}
