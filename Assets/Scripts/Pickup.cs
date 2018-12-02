using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {
	public Rigidbody Body;

	private bool PickedUp = false;
	private Vector3 OriginalPosition;
	private Quaternion OriginalRotation;
	private Transform Target;

	private void Update() {
		if (PickedUp) {
			if (Target == null) {
				transform.position = Vector3.Lerp(transform.position, OriginalPosition, 20f * Time.deltaTime);
				transform.rotation = Quaternion.Lerp(transform.rotation, OriginalRotation, 20f * Time.deltaTime);
				if ((transform.position - OriginalPosition).magnitude < 0.01f && Quaternion.Angle(transform.rotation, OriginalRotation) < 1) {
					transform.position = OriginalPosition;
					transform.rotation = OriginalRotation;
					RestoreState();
				}
			} else if (transform.parent != Target) {
				transform.position = Vector3.Lerp(transform.position, Target.position, 20f * Time.deltaTime);
				transform.rotation = Quaternion.Lerp(transform.rotation, Target.rotation, 20f * Time.deltaTime);
			}
		}
	}

	private void RestoreState() {
		Body.isKinematic = false;
		Body.detectCollisions = true;
		PickedUp = false;
	}

	public Vector3 GetOriginalPosition() {
		return OriginalPosition;
	}

	public void PickUp(Transform target) {
		OriginalPosition = transform.position;
		OriginalRotation = transform.rotation;
		Target = target;
		PickedUp = true;
		Body.detectCollisions = false;
		Body.isKinematic = true;
	}

	// Puts pickup down where it used to be
	public void PutDown() {
		Target = null;
	}

	// Drops the pickup on the spot
	public void ThrowDown() {
		Vector3 Force = Target.forward;
		Target = null;
		RestoreState();
		Body.AddForce(Force, ForceMode.Impulse);
		PickedUp = false;
	}
}
