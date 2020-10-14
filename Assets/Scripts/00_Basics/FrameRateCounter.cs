using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrameRateCounter : MonoBehaviour
{
	[SerializeField]
	TextMeshProUGUI display = default;

	private void Update() {
		float frameDuration = Time.unscaledDeltaTime;
		display.SetText("FPS\n{0:0}\n000\n000", 1f / frameDuration);
	}
}
