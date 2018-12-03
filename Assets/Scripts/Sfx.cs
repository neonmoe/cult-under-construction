using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sfx : MonoBehaviour {
	public AudioClip[] Clips;

	private AudioSource Source;
	private int Index = -1;
	private bool InitializedIndex = false;

	// This is mostly just a way to let someone who spawns a gameobject with this component
	// trigger the index rolling without waiting for Start().
	public void InitializeIndex(int index) {
		Index = index;
		if (Index < 0 || Index >= Clips.Length) {
			Index = Random.Range(0, Clips.Length);
		}
		InitializedIndex = true;
	}

	private void Start() {
		Source = GetComponent<AudioSource>();
		if (!InitializedIndex) InitializeIndex(Index);
		Source.clip = Clips[Index];
		Source.Play();
	}

	private void Update() {
		if (!Source.isPlaying) {
			Destroy(gameObject);
		}
	}
}
