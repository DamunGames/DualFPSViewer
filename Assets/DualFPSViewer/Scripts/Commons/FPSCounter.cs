using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamunGames.DualFPSViewer
{
	public class FPSCounter : MonoBehaviour
	{
		public float FPS { get; private set; }

		IntervalUpdater _intervalUpdater;
		int _frames;

		void Start()
		{
			_intervalUpdater = new(UpdateFPS);
		}

		void Update()
		{
			_frames++;
			_intervalUpdater.Update(Time.deltaTime);
		}

		void UpdateFPS(float elapsedTime)
		{
			FPS = _frames / elapsedTime;
			_frames = 0;
		}
	}
}