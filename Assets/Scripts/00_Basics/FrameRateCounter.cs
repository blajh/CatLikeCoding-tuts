using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrameRateCounter : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI display = default;
	[SerializeField, Range(0.1f, 2f)] float sampleDuration = .25f;

	int frames;
	float duration, worstDuration, bestDuration = float.MaxValue;

	void Update() {
		float frameDuration = Time.unscaledDeltaTime;
		frames += 1;
		duration += frameDuration;

		if (frameDuration < bestDuration) {
			bestDuration = frameDuration;
		}

		if (frameDuration > worstDuration) {
			worstDuration = frameDuration;
		}

		if (duration >= sampleDuration) {
			display.SetText(
				"FPS\nB:{0:0}\nA:{1:0}\nW:{2:0}",
				1f / bestDuration,
				frames / duration,
				1f / worstDuration);
			frames = 0;
			duration = 0f;
			bestDuration = float.MaxValue;
			worstDuration = 0f;
		}
	}
}
