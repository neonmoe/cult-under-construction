using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTeleporter : MonoBehaviour {
	public DoorTeleporter LinkedDoorTeleporter;
	public GameObject TeleportSoundPrefab;

	private float CooldownTime = -100;

	private void OnTriggerEnter(Collider other) {
		Player Player = other.GetComponent<Player>();
		CharacterController Character = other.GetComponent<CharacterController>();
		if (Player != null && Character && Time.time - CooldownTime > 0.2f) {
			Player.transform.rotation *= Quaternion.FromToRotation(transform.forward, LinkedDoorTeleporter.transform.forward);
			Player.transform.Rotate(0, 180, 0);
			Player.transform.position = LinkedDoorTeleporter.transform.TransformPoint(transform.InverseTransformPoint(Player.transform.position));
			LinkedDoorTeleporter.CooldownTime = Time.time;
			Instantiate(TeleportSoundPrefab, LinkedDoorTeleporter.transform);
		}
	}
}
