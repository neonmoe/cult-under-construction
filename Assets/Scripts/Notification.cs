using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour {
	public Text Text;
	public float NotificationLength = 2f;
	public float NotificationFadeInLength = 0.2f;
	public float NotificationFadeOutLength = 0.5f;
	public Vector2 TargetPos;

	private float NotifyTime = -100;
	private Vector2 InitialPosition;

	private void Start() {
		InitialPosition = Text.rectTransform.anchoredPosition;
	}

	private void Update() {
		float Alpha = 0f;
		if (Time.time < NotifyTime) {
			Alpha = Mathf.Clamp(Mathf.Pow((Time.time - (NotifyTime - NotificationLength)) / NotificationFadeInLength, 2), 0, 1);
		} else {
			Alpha = 1f - Mathf.Clamp(Mathf.Pow((Time.time - NotifyTime) / NotificationFadeOutLength, 2), 0, 1);
		}
		Color Color = Text.color;
		Color.a = Alpha;
		Text.color = Color;
		Text.rectTransform.anchoredPosition = InitialPosition + (TargetPos - InitialPosition) * Alpha;
	}

	public void Notify(string message, float length) {
		Text.text = message;
		NotificationLength = length;
		NotifyTime = Time.time + NotificationLength;
	}
}
