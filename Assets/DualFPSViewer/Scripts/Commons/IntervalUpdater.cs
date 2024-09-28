using System;
using System.Collections.Generic;
using UnityEngine;

namespace DamunGames.DualFPSViewer
{
	public class IntervalUpdater
	{
		const float UpdateIntervalMin = 0.001f;

		readonly Action _onUpdate;
		readonly Action<float> _onUpdateElapsedTime;

		float _updateInterval = 1.0f;
		float _time;

		public IntervalUpdater(Action onUpdate)
		{
			_onUpdate = onUpdate;
		}

		public IntervalUpdater(Action<float> onUpdate)
		{
			_onUpdateElapsedTime = onUpdate;
		}

		public void Update(float deltaTime)
		{
			_time += deltaTime;
			if (_time >= _updateInterval) {
				_onUpdate?.Invoke();
				_onUpdateElapsedTime?.Invoke(_time);
				_time %= _updateInterval;
			}
		}

		public void SetUpdateInterval(float updateInterval)
		{
			_updateInterval = updateInterval;
			if (_updateInterval <= UpdateIntervalMin) {
				_updateInterval = UpdateIntervalMin;
			}
		}
	}
}
