using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrameRateCounter : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI display = default;
	[SerializeField, Range(0.1f, 2f)] float sampleDuration = .25f;

	int frames;
	float duration;

	void Update() {
		float frameDuration = Time.unscaledDeltaTime;
		frames += 1;
		duration += frameDuration;

		if (duration >= sampleDuration) {
			display.SetText("FPS\n{0:0}\n000\n000", frames / duration);
			frames = 0;
			duration = 0f;
		}
	}
}
