using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DamunGames.DualFPSViewer
{
	public class MainScene : MonoBehaviour
	{
		[SerializeField] FPSCounter FPSCounter;
		[SerializeField] Slider HalfSplitSlider;
		[SerializeField] HalfScreen FirstHalfScreen;
		[SerializeField] HalfScreen SecondHalfScreen;

		[Serializable]
		public class HalfScreen
		{
			const int MaxFPS = 1000;

			public bool IsFirstHalf;
			public Camera Camera;
			public RenderTexture RenderTexture;
			public Image PreviewMaskImage;
			public WebGLNativeInputField FPSInputField;

			IntervalUpdater _intervalUpdater;

			public void Initialize(int defalutFPS)
			{
				_intervalUpdater = new(UpdateWithTargetFPS);
				SetUpdateInterval(defalutFPS);

				if (FPSInputField != null) {
					FPSInputField.onValueChanged.AddListener(OnEndEditFPSInputField);
				}

				Render();
			}

			public void Update(float deltaTime)
			{
				_intervalUpdater?.Update(deltaTime);
			}

			void UpdateWithTargetFPS() => Render();

			void Render()
			{
				if (Camera != null) {
					Camera.enabled = true;
					Camera.Render();
					Camera.enabled = false;
				}
			}

			public void SetRate(float t)
			{
				if (!IsFirstHalf) {
					t = 1.0f - t;
				}
				if (PreviewMaskImage != null) {
					PreviewMaskImage.fillAmount = t;
				}
			}

			public void SetScreenSize(int width, int height)
			{
				if (RenderTexture != null) {
					// Sizeを反映
					RenderTexture.Release();
					RenderTexture.width = width;
					RenderTexture.height = height;
					RenderTexture.Create();

					// Cameraの再計算
					if (Camera != null) {
						Camera.enabled = false;
						Camera.enabled = true;
					}
				}
			}

			void OnEndEditFPSInputField(string text)
			{
				if (int.TryParse(text, out int result)) {
					// 入力されたFPS値を更新インターバルに設定
					SetUpdateInterval(result);
				}
			}

			void SetUpdateInterval(int fps)
			{
				if (fps < 1) fps = 1;
				if (fps >= MaxFPS) fps = MaxFPS;

				float interval = 1.0f / fps;
				_intervalUpdater.SetUpdateInterval(interval);

				if (FPSInputField != null) {
					FPSInputField.text = string.Format("{0:d}", fps);
				}
			}
		}

		readonly List<HalfScreen> _halfScreens = new();

		int _screenWidth;
		int _screenHeight;
		int _refreshRate;

		IntervalUpdater _intervalUpdater;

		const int LogSize = 36;
		const int LogMax = 5;
		readonly List<string> _logs = new();

		void Start()
		{
			if (FirstHalfScreen != null) {
				_halfScreens.Add(FirstHalfScreen);
			}
			if (SecondHalfScreen != null) {
				_halfScreens.Add(SecondHalfScreen);
			}
			int refreshRate = GetRefreshRate();
			foreach (var halfScreen in _halfScreens) {
				halfScreen.Initialize(halfScreen.IsFirstHalf ? refreshRate : refreshRate / 2);
			}

			if (HalfSplitSlider != null) {
				foreach (var halfScreen in _halfScreens) {
					HalfSplitSlider.onValueChanged.AddListener(halfScreen.SetRate);
				}
			}

			UpdateScreenSize();
			UpdateRefreshRate();

			_intervalUpdater = new(IntervalUpdate);
			_intervalUpdater.SetUpdateInterval(1);
		}

		void Update()
		{
			float deltaTime = Time.deltaTime;

			_intervalUpdater.Update(deltaTime);

			foreach (var halfScreen in _halfScreens) {
				halfScreen.Update(deltaTime);
			}
		}

		void IntervalUpdate()
		{
			if (_screenWidth != Screen.width || _screenHeight != Screen.height) {
				UpdateScreenSize();
			}
			UpdateRefreshRate();
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

			foreach (var halfScreen in _halfScreens) {
				halfScreen.SetScreenSize(_screenWidth, _screenHeight);
			}
		}

		void UpdateRefreshRate()
		{
			_refreshRate = GetRefreshRate();
		}

		#region Log
		void AddLog(string log, bool isDebugLog = true)
		{
			if (isDebugLog) Debug.Log(log);
			_logs.Add(log);
		}

		void OnGUI()
		{
			GUI.skin.label.fontSize = LogSize;
			GUI.skin.label.fixedHeight = LogSize + 6.0f;

			float width = _screenWidth;
			float height = LogSize;
			Rect position = new(0.0f, _screenHeight - (height * LogMax), width, height);

			if (FPSCounter != null) {
				GUI.Label(position, string.Format("FPS: {0:0.}", FPSCounter.FPS));
				position.y += height;
			}

			GUI.Label(position, $"RefreshRate: {_refreshRate}");
			position.y += height;

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
}