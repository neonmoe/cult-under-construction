using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable {
	public Door LinkedDoor;
	public Transform DoorHinge;
	public bool IsOpen = false;
	public float OpenRotation = 90;
	public float ClosedRotation = 0;
	public GameObject TeleportSoundPrefab;
	public GameObject DoorOpenSfxPrefab;
	public GameObject DoorCloseSfxPrefab;
	public Transform DoorFxPosition;

	private float TargetRotation = 0;

	private void Start() {
		DoorTeleporter Teleporter = GetComponentInChildren<DoorTeleporter>();
		Teleporter.LinkedDoorTeleporter = LinkedDoor.GetComponentInChildren<DoorTeleporter>();
		Teleporter.TeleportSoundPrefab = TeleportSoundPrefab;
	}

	private void Update() {
		if (IsOpen) {
			TargetRotation = OpenRotation;
		} else if (!IsOpen) {
			TargetRotation = ClosedRotation;
		}
		Vector3 NewEulers = DoorHinge.localEulerAngles;
		NewEulers.z = Mathf.Lerp(NewEulers.z, TargetRotation, 10f * Time.deltaTime);
		DoorHinge.localEulerAngles = NewEulers;
	}

	private void SetOpen(bool open) {
		IsOpen = open;
		LinkedDoor.IsOpen = open;
	}

	public void Interact(GameObject equippedItem) {
		IsOpen = !IsOpen;
		LinkedDoor.SetOpen(IsOpen);
		if (IsOpen) {
			Instantiate(DoorOpenSfxPrefab, DoorFxPosition.position, new Quaternion());
		} else {
			Instantiate(DoorCloseSfxPrefab, DoorFxPosition.position, new Quaternion());
		}
	}
}
