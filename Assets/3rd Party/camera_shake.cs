using UnityEngine;
using System.Collections;

public class camera_shake : MonoBehaviour
{

	// How long the object should shake for.
	private float shakeDuration = 0f;

	// Amplitude of the shake. A larger value shakes the camera harder.
	private float shakeAmount = 3f;
	private float decreaseFactor = 15.0f;

	private Vector3 originalPos;

	void Awake()
	{
	}

	void OnEnable()
	{
		originalPos = transform.localPosition;
	}

	void Update()
	{
		if (shakeDuration > 0)
		{
			transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

			shakeDuration -= Time.deltaTime * decreaseFactor;
		}
		else
		{
			shakeDuration = 0f;
			//transform.localPosition = originalPos;
		}
	}

	public void Shake(float duration) {
		shakeDuration = duration;
	}
}