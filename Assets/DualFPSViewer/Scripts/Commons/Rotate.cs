using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamunGames.DualFPSViewer
{
	public class Rotate : MonoBehaviour
	{
		[SerializeField] float RotationSpeed = 45.0f;

		Transform _transform;

		void Start()
		{
			_transform = transform;
		}

		void Update()
		{
			float rotationAmount = RotationSpeed * Time.deltaTime;
			_transform.Rotate(rotationAmount * 0.5f, rotationAmount, 0.0f);
		}
	}
}
