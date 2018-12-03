using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SacrificialBowl : MonoBehaviour {
	public TextMeshPro NotificationText;
	public Vector3 NotificationOffset;
	public float NotificationLength;

	private float NotificationTime = -100;
	private Color InitialColor;
	private Vector3 InitialPosition;

	private void Start() {
		InitialColor = NotificationText.color;
		InitialPosition = NotificationText.transform.localPosition;
		NotificationText.color = new Color(0, 0, 0, 0);
	}

	private void Update() {
		if (Time.time - NotificationTime > NotificationLength) {
			Color Color = NotificationText.color;
			Vector3 Position = NotificationText.transform.localPosition;
			Position.y = Mathf.Lerp(Position.y, InitialPosition.y, 10f * Time.deltaTime);
			Color.a = Mathf.Lerp(Color.a, 0, 10f * Time.deltaTime);
			NotificationText.color = Color;
			NotificationText.transform.localPosition = Position;
		}
	}

	private void Notify(string message) {
		NotificationText.text = message;
		NotificationTime = Time.time;
		NotificationText.color = InitialColor;
		NotificationText.transform.localPosition = InitialPosition + NotificationOffset;
	}

	private void OnTriggerEnter(Collider collider) {
		Rigidbody Body = collider.attachedRigidbody;
		if (Body != null) {
			Examinable Examinable = Body.GetComponent<Examinable>();
			if (Examinable != null) {
				switch (Examinable.Name) {
				case ItemName.NiceFlower:
					Destroy(Body.gameObject);
					Notify("THAT IS A GOOD RED FLOWER.");
					return;
				case ItemName.AshesOfHappyVillage:
					Destroy(Body.gameObject);
					Notify("EXCELLENTLY BURNT. I HOPE NOBODY GOT HURT.");
					return;
				case ItemName.AnotherNote:
					Destroy(Body.gameObject);
					Notify("BREAKING NEWS:\nLAZY DEVELOPER FORGETS TO FIX GAME");
					return;
				}
			}
			switch (Random.Range(0, 7)) {
				case 0: Notify("SHAMEFUL SACRIFICE"); break;
				case 1: Notify("DOES THAT SEEM LIKE A GOOD OFFERING TO YOU?"); break;
				case 2: Notify("I KNOW YOU'RE NEW BUT HECK"); break;
				case 3: Notify("YOU SHOULD PROBABLY FIND A NEW DIETY"); break;
				case 4: Notify("YOU CAN DO BETTER"); break;
				case 5: Notify("WHAT WOULD I DO WITH THAT?"); break;
				case 6: Notify("THIS BOWL OF GREAT IMPORTANCE IS NOT A TRASHCAN"); break;
			}
			Body.AddTorque(new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)), ForceMode.VelocityChange);
			Body.AddForce(new Vector3(Random.Range(-1f, 1f), 5f, Random.Range(-1f, 1f)), ForceMode.VelocityChange);
		}
	}
}
