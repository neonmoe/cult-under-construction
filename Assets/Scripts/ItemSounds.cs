using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSounds : MonoBehaviour {
	public GameObject OpenSfxPrefab;
	public GameObject CloseSfxPrefab;
	public GameObject CollisionSfxPrefab;
	public float CollisionSfxCooldown = 0.2f;
	public float CollisionVelocityThreshold = 0.2f;
	public Rigidbody CollidingBody;

	private int PreviousOpenIndex = -1;
	private AudioSource CloseSource;
	private float LastCollision = -100;

	public void Open() {
		GameObject OpenSound = Instantiate(OpenSfxPrefab, transform.position, new Quaternion());
		Sfx Sfx = OpenSound.GetComponent<Sfx>();
		Sfx.InitializeIndex(-1);
	}

	public void Close() {
		GameObject OpenSound = Instantiate(CloseSfxPrefab, transform.position, new Quaternion());
		Sfx Sfx = OpenSound.GetComponent<Sfx>();
		Sfx.InitializeIndex(PreviousOpenIndex);
	}

	private void OnCollisionEnter(Collision collision) {
		if (Time.time - LastCollision > CollisionSfxCooldown && CollidingBody.velocity.magnitude > CollisionVelocityThreshold) {
			Instantiate(CollisionSfxPrefab, transform.position, new Quaternion());
			LastCollision = Time.time;
		}
	}
}
